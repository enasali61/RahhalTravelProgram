using System.Globalization;
using System.Security.Claims;
using AutoMapper;
using Domain.Contracts;
using Domain.Entities;
using Domain.Entities.TripAndPlaces;
using Domain.Exceptions;
using GTranslate;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Abstraction;
using Shared.DTOs;

namespace Services
{
    public class BasketService(IBasketRepository _basketRepository,
        IMapper mapper, IUnitOfWork _unitOfWork, UserManager<Users> _userManager, IHttpContextAccessor _httpContextAccessor) :
         BaseService(_userManager, _httpContextAccessor),IBasketService
    {
        private async Task<BasketDto> EnrichBasketAsync(CustomerBasket basket, string? language = null, int travelersCount = 1)
        {
            // 1. Fetch all needed data once
            var allPlaces = await _unitOfWork.Set<Places>()
                .Include(p => p.Images)
                .ToListAsync();
            var places = allPlaces.ToDictionary(p => p.Id);

            var allTrips = await _unitOfWork.Set<Trip>().ToListAsync();
            var trips = allTrips.ToDictionary(t => t.Id);
            // 2. Get user + resolve language
            var user = await GetCurrentUserAsync();
            bool isEgyptian = user.IsEgyptian;
            bool isStudent = user.IsStudent;

            string lang = (language == "ar" || language == "en")
                ? language
                : (isEgyptian ? "ar" : "en");

            var enrichedItems = new List<BasketItemsDto>();

            foreach (var item in basket.Items)
            {
                string name = "";
                string description = "";
                string imageUrl = "";
                string city = "";
                decimal price = 0;

                if (item.Type == "Place" && places.TryGetValue(item.EntityId, out var place))
                {
                    name = lang == "ar"
                        ? (!string.IsNullOrEmpty(place.NameAr) ? place.NameAr : place.NameEn)
                        : place.NameEn;

                    description = lang == "ar"
                        ? (place.HistoricalBackGroundAr ?? place.HistoricalBackGroundEn ?? "")
                        : (place.HistoricalBackGroundEn ?? "");

                    city = lang == "ar"
                        ? (!string.IsNullOrEmpty(place.CityAr) ? place.CityAr : place.CityEn)
                        : place.CityEn;

                    imageUrl = place.Images?
                        .FirstOrDefault(img => img.IsMain)?.ImageUrl
                        ?? place.Images?.FirstOrDefault()?.ImageUrl
                        ?? "";

                    // use snapshot if available (price was frozen at add time)
                    price = item.PriceSnapshot ?? (isEgyptian
                        ? (isStudent ? place.PriceEgStudent : place.PriceEgAdult)
                        : (isStudent ? place.PriceForeignStudent : place.PriceForeignAdult));
                }
                else if (item.Type == "Trip" && trips.TryGetValue(item.EntityId, out var trip))
                {
                    name = trip.TripName;
                    description = trip.Notes ?? "";
                    price = item.PriceSnapshot ?? trip.TotalBudget;
                }
                else
                {
                    name = "Unavailable";
                    price = 0;
                }

                enrichedItems.Add(new BasketItemsDto
                {
                    Id = item.Id,
                    Type = item.Type,
                    EntityId = item.EntityId,
                    Quantity = item.Quantity,
                    AddedAt = item.AddedAt,
                    Name = name,
                    Description = description,
                    ImageUrl = imageUrl,
                    City = city,
                    Price = price
                });
            }
            var sortedEnrichedItems = enrichedItems.OrderBy(i => i.AddedAt).ToList();
            int numberOfPlaces = enrichedItems.Count(i => i.Type == "Place");
            decimal baseSingleTotal = enrichedItems.Sum(i => i.Price * i.Quantity);
            decimal groupTotalCost = baseSingleTotal * travelersCount;
            return new BasketDto
            {
                Id = basket.Id,
                Items = sortedEnrichedItems,
                TotalCost = groupTotalCost, // price × qty per item, summed
                NumberOfPlaces = numberOfPlaces,
                DurationDays = numberOfPlaces > 0 ? 1 : 0
            };
        }
        public async Task<BasketDto> GetBasketAsync(string? language, int travelersCount)
        {
            var userId = await GetCurrentUserIdAsync();
            var basket = await _basketRepository.GetBasketAsync(userId.ToString());
            if (basket == null)
                throw new BasketNotFoundException(userId);
            return await EnrichBasketAsync(basket,language,travelersCount);
        }

        // 2. Add item to current user's basket
        public async Task<BasketDto> AddItemAsync(AddToBasketDto itemDto, string? language, int travelersCount)
        {
            var user = await GetCurrentUserAsync();
            var basketId = user.Id.ToString();

            // Get or create basket
            var basket = await _basketRepository.GetBasketAsync(basketId);
            if (basket == null)
            {
                basket = new CustomerBasket { Id = basketId, Items = new List<BasketItems>() };
            }
            var preferredDate = DateTime.Today.AddDays(1);
            var quantity = 1;

            decimal? calculatedPrice = null;
            if (itemDto.Type == "Place")
            {
                var place = await _unitOfWork.Set<Places>().FirstOrDefaultAsync(t => t.Id == itemDto.EntityId );
                if (place == null)
                    throw new Exception($"Place with ID {itemDto.EntityId} not found");

                calculatedPrice = user.IsEgyptian
                    ? (user.IsStudent ? place.PriceEgStudent : place.PriceEgAdult)
                    : (user.IsStudent ? place.PriceForeignStudent : place.PriceForeignAdult);
            }
            else if (itemDto.Type == "Trip")
            {
                var trip = await _unitOfWork.Set<Trip>().FirstOrDefaultAsync(t => t.Id == itemDto.EntityId&& t.IsTemplate == true);
                if (trip == null)
                    throw new Exception($"Trip with ID {itemDto.EntityId} not found");
                calculatedPrice = trip.TotalBudget;
            }
            else
            {
                throw new Exception($"Invalid type '{itemDto.Type}'");
            }

            // Create new basket item
            var newItem = new BasketItems
            {
                Id = basket.Items.Any() ? basket.Items.Max(i => i.Id) + 1 : 1,
                Type = itemDto.Type,
                EntityId = itemDto.EntityId,
                Quantity = quantity,
                AddedAt = DateTime.UtcNow,
                PreferredDate = preferredDate,
                PriceSnapshot = calculatedPrice
            };
            basket.Items.Add(newItem);

            var updatedBasket = await _basketRepository.UpdateBasketAsync(basket);
            if (updatedBasket == null)
                throw new Exception("Failed to update basket");

            return await EnrichBasketAsync(updatedBasket,language,travelersCount);
        }

        // 3. Remove item from basket
        public async Task RemoveItemAsync(int itemId)
        {
            var userId = await GetCurrentUserIdAsync();
            var basket = await _basketRepository.GetBasketAsync(userId.ToString());
            if (basket == null)
                throw new BasketNotFoundException(userId);

            var item = basket.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                throw new Exception($"Item with ID {itemId} not found in basket");

            basket.Items.Remove(item);
            await _basketRepository.UpdateBasketAsync(basket);
        }

        // 4. Clear basket
        public async Task ClearBasketAsync()
        {
            var userId = await GetCurrentUserIdAsync();
            await _basketRepository.DeleteBasketAsync(userId.ToString());
        }

        // 5. Update basket (if needed – usually not called directly)
        public async Task<BasketDto?> UpdateBasketAsync(BasketDto basket)
        {
            // This method is rarely used; you'd normally use Add/Remove.
            // Keep it only if you need to replace the entire basket.
            var userId = await GetCurrentUserIdAsync();
            if (basket.Id != userId.ToString())
                throw new UnauthorizedAccessException("Cannot update another user's basket");

            var customerBasket = mapper.Map<CustomerBasket>(basket);
            var updatedBasket = await _basketRepository.UpdateBasketAsync(customerBasket);
            if (updatedBasket == null)
                throw new Exception("Failed to update basket");

            return await EnrichBasketAsync(updatedBasket);
        }

        // 6. Enrichment helper (private)
    }
    
}

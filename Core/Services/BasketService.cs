using AutoMapper;
using Domain.Contracts;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Services.Abstraction;
using Shared.DTOs;

namespace Services
{
    public class BasketService(IBasketRepository basketRepository,
        IMapper mapper, IUnitOfWork unitOfWork) : IBasketService
    {
        public async Task<BasketDto> CreateOrUpdateBasketAsync(BasketDto basket)
        {
            var customerBasket = mapper.Map<CustomerBasket>(basket);
            var updatedBasket = await basketRepository.UpdateBasketAsync(customerBasket);
            return updatedBasket is null ? throw new Exception("can't update basket") : await GetBasketAsync(updatedBasket.Id);
        }

        public async Task<bool> DeleteBasketAsync(string basketId)
        => await basketRepository.DeleteBasketAsync(basketId);

        public async Task<BasketDto> GetBasketAsync(string basketId)
        {
            var basket = await basketRepository.GetBasketAsync(basketId);
            if (basket is null)
                throw new BasketNotFoundException(basketId);

            // Build dictionaries of places and trips (with all data loaded once)
            var places = new Dictionary<int, Places>();
            var trips = new Dictionary<int, Trip>();

            // Fetch all places (with images) – suitable for small datasets
            var allPlaces = await unitOfWork.Set<Places>()
                .Include(p => p.Images)
                .ToListAsync();
            places = allPlaces.ToDictionary(p => p.Id);

            // Fetch all trips (if you have many trips, consider filtering later)
            var allTrips = await unitOfWork.Set<Trip>().ToListAsync();
            trips = allTrips.ToDictionary(t => t.Id);

            // Build enriched items list
            var enrichedItems = new List<BasketItemsDto>();

            foreach (var item in basket.Items)
            {
                string name = "";
                string description = "";
                string imageUrl = "";
                decimal price = 0;

                if (item.Type == "Place" && places.TryGetValue(item.EntityId, out var place))
                {
                    name = place.NameAr;
                    description = place.Description ?? "";
                    imageUrl = place.Images?.FirstOrDefault()?.ImageUrl ?? "";
                    price = item.PriceSnapshot ?? place.PriceEgAdult;
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
                    Notes = item.Notes,
                    AddedAt = item.AddedAt,
                    Name = name,
                    Description = description,
                    ImageUrl = imageUrl,
                    Price = price
                });
            }

            return new BasketDto
            {
                Id = basket.Id,
                Items = enrichedItems,
                // Total = enrichedItems.Sum(i => (i.Price ?? 0) * i.Quantity)
            };
        }
    }
    
}

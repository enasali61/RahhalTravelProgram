using System.Security.Claims;
using AutoMapper;
using Domain.Contracts;
using Domain.Entities;
using Domain.Entities.TripAndPlaces;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Abstraction;
using Services.Specifications;
using Shared;
using Shared.DTOs.PlacesDto;
using Shared.DTOs.TripDto;

namespace Services
{
    public class TripService(IUnitOfWork _unitOfWork, IMapper _mapper, IBasketRepository _basketRepository,
        IHttpContextAccessor _httpContextAccessor, UserManager<Users> _userManager, ITranslationServices translationService) :
        BaseService(_userManager, _httpContextAccessor), ITripService
    {
       
        private async Task<PlacesResultDTO> BuildPlaceDtoAsync(Places place, string language, Users? user)
        {
            var dto = _mapper.Map<PlacesResultDTO>(place);
            bool needsSave = false;

            if (language == "ar")
            {
                dto.Name = !string.IsNullOrEmpty(place.NameAr) ? place.NameAr : place.NameEn;
                dto.City = !string.IsNullOrEmpty(place.CityAr) ? place.CityAr : place.CityEn;
                dto.Category = !string.IsNullOrEmpty(place.Category?.NameAr) ? place.Category.NameAr : place.Category?.NameEn;

                if (!string.IsNullOrEmpty(place.
                    HistoricalBackGroundAr) && !string.IsNullOrEmpty(place.VisitingTimeAr))
                {
                    dto.HistoricalBackGround = place.HistoricalBackGroundAr;
                    dto.VisitingTime = place.VisitingTimeAr;
                }

                else if (!string.IsNullOrEmpty(place.HistoricalBackGroundEn) && !string.IsNullOrEmpty(place.VisitingTimeEn))
                {
                    var translatedHistoricalBackGround = await translationService.TranslateToArabicAsync(place.HistoricalBackGroundEn);
                    var translatedVisitingTime = await translationService.TranslateToArabicAsync(place.VisitingTimeEn);
                    dto.HistoricalBackGround = translatedHistoricalBackGround;
                    dto.VisitingTime = translatedVisitingTime;
                    place.HistoricalBackGroundAr = translatedHistoricalBackGround;
                    place.VisitingTimeAr = translatedVisitingTime;
                    needsSave = true;
                }
            }
            else
            {
                dto.Name = place.NameEn;
                dto.City = place.CityEn;
                dto.HistoricalBackGround = place.HistoricalBackGroundEn;
                dto.VisitingTime = place.VisitingTimeEn;
                dto.Category = place.Category.NameEn;
            }

            dto.Price = user != null
                ? (user.IsEgyptian ? (user.IsStudent ? place.PriceEgStudent : place.PriceEgAdult)
                                    : (user.IsStudent ? place.PriceForeignStudent : place.PriceForeignAdult))
                : place.PriceForeignAdult;

            if (needsSave)
                await _unitOfWork.SaveChangesAsync();

            return dto;
        }

        public async Task<PaginatedResult<TripDto>> GetAllTripsAsync(TripSpecParams tripSpecParams)
        {
            var trips = await _unitOfWork.GetRepository<Trip>().GetAllAsync(new TripSpec(tripSpecParams));
            var tripRes = _mapper.Map<IEnumerable<TripDto>>(trips);

            var count = tripRes.Count();
            var totalCount = await _unitOfWork.GetRepository<Trip>().CountAsync(new TripSpec(tripSpecParams));

            var result = new PaginatedResult<TripDto>
            (
                tripSpecParams.PageIndex,
                count,
                totalCount,
                tripRes
            );
            return result;
        }

        public async Task<TripDto> GetTripByIdAsync(int id, string? language = null)
        {
            var trip = await _unitOfWork.Set<Trip>()
                .Include(t => t.TripPlaces)
                    .ThenInclude(tp => tp.Place)
                        .ThenInclude(p => p.Images)
                .Include(t => t.TripPlaces)
                    .ThenInclude(tp => tp.Place)
                        .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(t => t.Id == id);

           

            var user = await GetCurrentUserAsync();
            string resolvedLanguage = (language == "ar" || language == "en")
                ? language
                : (user?.IsEgyptian == true ? "ar" : "en");

            var tripDto = _mapper.Map<TripDto>(trip);
            bool needsSave = false;

            for (int j = 0; j < trip.TripPlaces.Count; j++)
            {
                var tripPlace = trip.TripPlaces.ElementAt(j);
                var tripPlaceDto = tripDto.TripPlaces[j];

                if (tripPlace.Place == null) continue;

                var enrichedPlace = await BuildPlaceDtoAsync(tripPlace.Place, resolvedLanguage, user);
                tripPlaceDto.Place = enrichedPlace;

                if (enrichedPlace.HistoricalBackGround != null && tripPlace.Place.HistoricalBackGroundAr == null && resolvedLanguage == "ar")
                    needsSave = true;
            }

            if (needsSave)
                await _unitOfWork.SaveChangesAsync();

            return tripDto;
        }

        public async Task<IEnumerable<object>> GetTripDailyScheduleAsync(int tripId)
        {
            var trip = await _unitOfWork.GetRepository<Trip>().GetByIdAsync(tripId);
            var tripPlaces = await GetTripPlacesAsync(tripId);

            var dailySchedule = tripPlaces
                .GroupBy(tp => tp.VisitDate)
                .Select(g => new
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    DayNumber = (g.Key - trip.StartDate).Days + 1,
                    Places = g.OrderBy(tp => tp.VisitOrder).ToList(),
                    TotalCost = g.Sum(tp => tp.EstimatedCost)
                })
                .OrderBy(d => d.Date)
                .ToList();

            return dailySchedule;
        }

        public async Task<decimal> CalculateTripCostAsync(int tripId)
        {
            return await _unitOfWork.Set<TripPlace>()
            .Where(tp => tp.TripId == tripId)
            .SumAsync(tp => tp.EstimatedCost);
        }
        public async Task<IEnumerable<TripDto>> GetUserTripsAsync(string? language = null)
        {
            var user = await GetCurrentUserAsync();

            var trips = await _unitOfWork.GetRepository<Trip>().GetAllAsync();

            var userTrips = trips
                .Where(t => t.UserId == user.Id || t.IsTemplate)
                .OrderByDescending(t => t.IsTemplate)
                .ThenBy(t => t.StartDate)
                .ToList();

            string resolvedLanguage = (language == "ar" || language == "en")
                ? language
                : (user.IsEgyptian ? "ar" : "en");

            var tripDtos = _mapper.Map<List<TripDto>>(userTrips);
            bool needsSave = false;

            for (int i = 0; i < userTrips.Count; i++)
            {
                var trip = userTrips[i];
                var tripDto = tripDtos[i];

                for (int j = 0; j < trip.TripPlaces.Count; j++)
                {
                    var tripPlace = trip.TripPlaces.ElementAt(j);
                    var tripPlaceDto = tripDto.TripPlaces[j];

                    if (tripPlace.Place == null) continue;

                    var enrichedPlace = await BuildPlaceDtoAsync(tripPlace.Place, resolvedLanguage, user);
                    tripPlaceDto.Place = enrichedPlace;

                    if (enrichedPlace.HistoricalBackGround != null && tripPlace.Place.HistoricalBackGroundAr == null && resolvedLanguage == "ar")
                        needsSave = true;
                }
            }

            if (needsSave)
                await _unitOfWork.SaveChangesAsync();

            return tripDtos;
        }

        public async Task<IEnumerable<TripDto>> GetPopularTripsAsync(int count, string? language = "en")
        {
            var trips = await _unitOfWork.GetRepository<Trip>().GetAllAsync();
            var popularTrips = trips
                .Where(t => t.IsTemplate)
                .OrderByDescending(t => t.TripPlaces.Count > 0 ? t.TotalBudget / t.TripPlaces.Count : 0)
                .Take(count)
                .ToList();

            // fetch user once — same pattern as GetAllPlacesAsync
            var user = await GetCurrentUserAsync();
            string resolvedLanguage = (language == "ar" || language == "en")
                ? language
                : (user?.IsEgyptian == true ? "ar" : "en");

            // map trips first
            var tripDtos = _mapper.Map<List<TripDto>>(popularTrips);

            // then enrich each place inside each trip
            bool needsSave = false;
            for (int i = 0; i < popularTrips.Count; i++)
            {
                var trip = popularTrips[i];
                var tripDto = tripDtos[i];

                for (int j = 0; j < trip.TripPlaces.Count; j++)
                {
                    var tripPlace = trip.TripPlaces.ElementAt(j);
                    var tripPlaceDto = tripDto.TripPlaces[j];

                    if (tripPlace.Place == null) continue;

                    // reuse the exact same BuildPlaceDtoAsync you use everywhere else
                    var enrichedPlace = await BuildPlaceDtoAsync(tripPlace.Place, resolvedLanguage, user);
                    tripPlaceDto.Place = enrichedPlace;

                    if (enrichedPlace.HistoricalBackGround != null && tripPlace.Place.HistoricalBackGroundAr == null && resolvedLanguage == "ar")
                        needsSave = true;
                }
            }

            if (needsSave)
                await _unitOfWork.SaveChangesAsync();

            return tripDtos;
        }
        public async Task<TripDto> ConfirmBasketAndCreateTripAsync(int userId)
        {
            var basket = await _basketRepository.GetBasketAsync(userId.ToString());
            if (basket == null || !basket.Items.Any())
                throw new InvalidOperationException("Basket is empty");


            var preferredDates = basket.Items
       .Where(i => i.PreferredDate.HasValue)
       .Select(i => i.PreferredDate.Value)
       .ToList();

            DateTime startDate = preferredDates.Any()
                ? preferredDates.Min()  // earliest date
                : DateTime.Today;

            var trip = new Trip
            {
                UserId = userId,
                TripName = "My Trip",
                StartDate = startDate,
                EndDate = startDate.AddDays(10),
                IsTemplate = false,
                Status = TripStatusOption.Planned,
                TotalBudget = 0
            };

            await _unitOfWork.GetRepository<Trip>().AddAsync(trip);
            await _unitOfWork.SaveChangesAsync(); // get trip.Id
            int order = 0;
            decimal total = 0;
            foreach (var item in basket.Items)
            {
                if (item.Type == "Place")
                {
                    var place = await _unitOfWork.Set<Places>().FindAsync(item.EntityId);
                    var tripPlace = new TripPlace
                    {
                        TripId = trip.Id,
                        PlaceId = item.EntityId,
                        VisitDate = item.PreferredDate ?? startDate,
                        VisitOrder = order++,
                        EstimatedCost = item.PriceSnapshot ?? place?.PriceEgAdult ?? 0,
                    };
                    await _unitOfWork.Set<TripPlace>().AddAsync(tripPlace);
                    total += tripPlace.EstimatedCost;
                }
                else if (item.Type == "Trip")
                {
                    var templateTrip = await _unitOfWork.Set<Trip>()
                        .Include(t => t.TripPlaces)
                        .FirstOrDefaultAsync(t => t.Id == item.EntityId && t.IsTemplate == true);
                    if (templateTrip != null)
                    {
                        if (!templateTrip.TripPlaces.Any())
                            Console.WriteLine($"⚠️ Template trip {templateTrip.Id} has no places attached.");

                        foreach (var tp in templateTrip.TripPlaces)
                        {
                            var newTp = new TripPlace
                            {
                                TripId = trip.Id,
                                PlaceId = tp.PlaceId,
                                VisitDate = tp.VisitDate,
                                VisitOrder = order++,
                                EstimatedCost = tp.EstimatedCost,
                                Notes = tp.Notes
                            };
                            await _unitOfWork.Set<TripPlace>().AddAsync(newTp);
                            total += newTp.EstimatedCost;
                        }
                    }
                }
            }
            var maxVisitDate = await _unitOfWork.Set<TripPlace>().Where(tp => tp.TripId == trip.Id)
                .MaxAsync(tp => tp.VisitDate);

            // trip.EndDate = maxVisitDate > startDate ? maxVisitDate : startDate.AddDays(1);
            trip.TotalBudget = total;
            await _unitOfWork.SaveChangesAsync(); // save all trip places

            // Clear basket
            await _basketRepository.DeleteBasketAsync(userId.ToString());

            return await GetTripByIdAsync(trip.Id);
        }

        public async Task<TripDto> CreateTripAsync(CreateTripDto createDto)
        {
            // 1. Fetch the user from the database to check their type/role
            var user = await GetCurrentUserAsync();
            var userId = await GetCurrentUserIdAsync();
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");
                     
            
            var trip = new Trip
            {
                UserId = userId,
                TripName = createDto.TripName,
                StartDate = createDto.StartDate,
                EndDate = createDto.EndDate,
                IsTemplate = user.UserType == "Admin",
                Status = TripStatusOption.Planned,
                TotalBudget = createDto.TotalBudget,
                TravelersCount = createDto.TravelersCount,
                Notes = createDto.Notes
            };

            await _unitOfWork.GetRepository<Trip>().AddAsync(trip);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<TripDto>(trip);
        }

        public async Task UpdateTripAsync(int id, UpdateTripDto updateDto, int currentUserId, bool isAdmin)
        {
            var trip = await _unitOfWork.GetRepository<Trip>().GetByIdAsync(id);
            if (trip == null)
                throw new KeyNotFoundException($"Trip {id} not found");

            // Authorization: user can edit own trip OR admin can edit any trip
            if (!isAdmin && trip.UserId != currentUserId)
                throw new UnauthorizedAccessException("You can only edit your own trips.");

            // Apply changes (only provided fields)
            if (!string.IsNullOrEmpty(updateDto.TripName))
                trip.TripName = updateDto.TripName;
            if (updateDto.StartDate.HasValue)
                trip.StartDate = updateDto.StartDate.Value;
            if (updateDto.EndDate.HasValue)
                trip.EndDate = updateDto.EndDate.Value;
            if (updateDto.TravelersCount.HasValue)
                trip.TravelersCount = updateDto.TravelersCount.Value;
            if (updateDto.Notes != null)
                trip.Notes = updateDto.Notes;
            // Status can be updated by admin or user? You decide, but I'll keep optional
            if (!string.IsNullOrEmpty(updateDto.Status))
            {
                if (Enum.TryParse<TripStatusOption>(updateDto.Status, true, out var status))
                    trip.Status = status;
            }

            _unitOfWork.GetRepository<Trip>().Update(trip);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteTripAsync(int id, int currentUserId, bool isAdmin)
        {
            var trip = await _unitOfWork.GetRepository<Trip>().GetByIdAsync(id);
            if (trip == null)
                throw new KeyNotFoundException($"Trip {id} not found");

            if (!isAdmin && trip.UserId != currentUserId)
                throw new UnauthorizedAccessException("You can only delete your own trips.");

            // Delete related TripPlaces first (cascade delete should handle it, but to be safe)
            var tripPlaces = await _unitOfWork.Set<TripPlace>()
                .Where(tp => tp.TripId == id)
                .ToListAsync();
            _unitOfWork.Set<TripPlace>().RemoveRange(tripPlaces);
            _unitOfWork.GetRepository<Trip>().Delete(trip);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<TripPlaceDto> AddPlaceToTripAsync(int tripId, AddPlaceToTripDto addDto, int currentUserId, bool isAdmin)
        {
            var trip = await _unitOfWork.GetRepository<Trip>().GetByIdAsync(tripId);
            if (trip == null)
                throw new KeyNotFoundException($"Trip {tripId} not found");

            // Only owner or admin can add places to a trip
            if (!isAdmin && trip.UserId != currentUserId)
                throw new UnauthorizedAccessException("You cannot modify this trip.");

            // Verify place exists
            var place = await _unitOfWork.GetRepository<Places>().GetByIdAsync(addDto.PlaceId);
            if (place == null)
                throw new KeyNotFoundException($"Place {addDto.PlaceId} not found");

            var tripPlace = new TripPlace
            {
                TripId = tripId,
                PlaceId = addDto.PlaceId,
                VisitDate = addDto.VisitDate,
                VisitOrder = addDto.VisitOrder,
                StartTime = addDto.StartTime,
                EndTime = addDto.EndTime,
                EstimatedCost = addDto.EstimatedCost,
                Notes = addDto.Notes,
                IsVisited = false
            };
            await _unitOfWork.Set<TripPlace>().AddAsync(tripPlace);

            // Update trip's total budget (add the new cost)
            trip.TotalBudget += addDto.EstimatedCost;
            _unitOfWork.GetRepository<Trip>().Update(trip);

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<TripPlaceDto>(tripPlace);
        }

        public async Task UpdateTripPlaceAsync(int tripId, int placeId, UpdateTripPlaceDto updateDto, int currentUserId, bool isAdmin)
        {
            var tripPlace = await _unitOfWork.Set<TripPlace>()
            .Include(tp => tp.Trip)
            .FirstOrDefaultAsync(tp => tp.TripId == tripId && tp.PlaceId == placeId);
            if (tripPlace == null)
                throw new KeyNotFoundException($"TripPlace {tripId}and{placeId} not found");

            var trip = tripPlace.Trip;
            if (!isAdmin && trip.UserId != currentUserId)
                throw new UnauthorizedAccessException("You cannot modify this trip.");

            // Adjust total budget if estimated cost changes
            if (updateDto.EstimatedCost.HasValue && updateDto.EstimatedCost.Value != tripPlace.EstimatedCost)
            {
                var oldCost = tripPlace.EstimatedCost;
                var newCost = updateDto.EstimatedCost.Value;
                trip.TotalBudget = trip.TotalBudget - oldCost + newCost;
                _unitOfWork.GetRepository<Trip>().Update(trip);
                tripPlace.EstimatedCost = newCost;
            }

            if (updateDto.VisitDate.HasValue)
                tripPlace.VisitDate = updateDto.VisitDate.Value;
            if (updateDto.VisitOrder.HasValue)
                tripPlace.VisitOrder = updateDto.VisitOrder.Value;
            if (updateDto.StartTime.HasValue)
                tripPlace.StartTime = updateDto.StartTime;
            if (updateDto.EndTime.HasValue)
                tripPlace.EndTime = updateDto.EndTime;
            if (updateDto.Notes != null)
                tripPlace.Notes = updateDto.Notes;
            if (updateDto.IsVisited.HasValue)
                tripPlace.IsVisited = updateDto.IsVisited.Value;

            _unitOfWork.Set<TripPlace>().Update(tripPlace);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemovePlaceFromTripAsync(int tripId, int placeId, int currentUserId, bool isAdmin)
        {
            var tripPlace = await _unitOfWork.Set<TripPlace>().Include(tp => tp.Trip)
                .FirstOrDefaultAsync(tp => tp.TripId == tripId && tp.PlaceId == placeId); ;
            if (tripPlace == null)
                throw new KeyNotFoundException($"TripPlace {tripId} and {placeId} not found");

            var trip = tripPlace.Trip;
            if (!isAdmin && trip.UserId != currentUserId)
                throw new UnauthorizedAccessException("You cannot modify this trip.");

            // Subtract the cost from trip's total budget
            trip.TotalBudget -= tripPlace.EstimatedCost;
            _unitOfWork.GetRepository<Trip>().Update(trip);

            _unitOfWork.Set<TripPlace>().Remove(tripPlace);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<TripPlaceDto>> GetTripPlacesAsync(int tripId)
        {
            var tripPlaces = await _unitOfWork.Set<TripPlace>()
            .Where(tp => tp.TripId == tripId)
            .Include(tp => tp.Place)
            .ToListAsync();

            return _mapper.Map<IEnumerable<TripPlaceDto>>(tripPlaces);
        }

        
    }
}

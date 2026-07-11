using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;
using Shared.DTOs.TripDto;

namespace Services.Abstraction
{
    public interface ITripService
    {   
        Task<PaginatedResult<TripDto>> GetAllTripsAsync(TripSpecParams tripSpecParams);
        Task<TripDto> GetTripByIdAsync(int id,string? language = null);
        //Task<IEnumerable<TripDto>> GetAllTemplateTripsAsync(string? language);
        Task<TripDto> CreateTripAsync(CreateTripDto createDto);
        Task UpdateTripAsync(int id, UpdateTripDto updateDto,int currentUserId, bool isAdmin);
        Task DeleteTripAsync(int id, int currentUserId, bool isAdmin);
        Task<IEnumerable<TripDto>> GetUserTripsAsync( string? language = null);

        Task<IEnumerable<TripDto>> GetPopularTripsAsync(int count, string? language = null);
        Task<TripDto> ConfirmBasketAndCreateTripAsync(int userId);
        Task<IEnumerable<TripPlaceDto>> GetTripPlacesAsync(int tripId);
        
        Task<TripPlaceDto> AddPlaceToTripAsync(int tripId, AddPlaceToTripDto addDto, int currentUserId, bool isAdmin);
        Task UpdateTripPlaceAsync(int tripId, int placeId, UpdateTripPlaceDto updateDto, int currentUserId, bool isAdmin);
        Task RemovePlaceFromTripAsync(int tripId, int placeId, int currentUserId, bool isAdmin);

        // Trip Statistics
        Task<decimal> CalculateTripCostAsync(int tripId);
        Task<IEnumerable<object>> GetTripDailyScheduleAsync(int tripId);
    }
}

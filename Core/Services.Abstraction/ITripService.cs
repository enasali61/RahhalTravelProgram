using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DTOs;

namespace Services.Abstraction
{
    public interface ITripService
    {
        
        Task<IEnumerable<TripDto>> GetAllTripsAsync();
        Task<TripDto> GetTripByIdAsync(int id);
        
        //Task<TripDto> CreateTripAsync(int userId, CreateTripDto createDto);
        //Task UpdateTripAsync(int id, UpdateTripDto updateDto);
        //Task DeleteTripAsync(int id);
        Task<IEnumerable<TripDto>> GetUserTripsAsync(int userId);

        
        Task<IEnumerable<TripPlaceDto>> GetTripPlacesAsync(int tripId);
        
        //Task<TripPlaceDto> AddPlaceToTripAsync(int tripId, AddPlaceToTripDto addDto);
        //Task UpdateTripPlaceAsync(int tripPlaceId, UpdateTripPlaceDto updateDto);
        //Task RemovePlaceFromTripAsync(int tripPlaceId);

        // Trip Statistics
        Task<decimal> CalculateTripCostAsync(int tripId);
        Task<IEnumerable<object>> GetTripDailyScheduleAsync(int tripId);
    }
}

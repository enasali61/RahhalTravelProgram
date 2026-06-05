using AutoMapper;
using Domain.Contracts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Services.Abstraction;
using Shared.DTOs;

namespace Services
{
    public class TripService(IUnitOfWork _unitOfWork, IMapper _mapper) : ITripService
    {

        public async Task<IEnumerable<TripDto>> GetAllTripsAsync()
        {
            var trips = await _unitOfWork.GetRepository<Trip>().GetAllAsync();
            var res = _mapper.Map<IEnumerable<TripDto>>(trips);

            return res;
        }

        public async Task<TripDto> GetTripByIdAsync(int id)
        {
            var trip = await _unitOfWork.GetRepository<Trip>().GetByIdAsync(id);
            return _mapper.Map<TripDto>(trip);
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

        public async Task<IEnumerable<TripPlaceDto>> GetTripPlacesAsync(int tripId)
        {
            var tripPlaces = await _unitOfWork.Set<TripPlace>()
             .Where(tp => tp.TripId == tripId)
             .Include(tp => tp.Place)
             .ToListAsync();

            return _mapper.Map<IEnumerable<TripPlaceDto>>(tripPlaces);

        }

        public async Task<IEnumerable<TripDto>> GetUserTripsAsync(int userId)
        {
            var trips = await _unitOfWork.GetRepository<Trip>().GetAllAsync();
            var userTrips = trips.Where(t => t.UserId == userId).ToList();

            return _mapper.Map<IEnumerable<TripDto>>(userTrips);
        }
    }
}

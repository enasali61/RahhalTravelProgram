using System;
using System.Security.Claims;
using Domain.Contracts;
using Domain.Entities;
using Domain.Entities.SubEntity;
using Domain.Entities.TripAndPlaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Abstraction;
using Shared.DTOs.UserDto;

namespace Services
{
    public class UserService(IUnitOfWork _unitOfWork, UserManager<Users> _userManager, IHttpContextAccessor _httpContextAccessor) :
        BaseService(_userManager, _httpContextAccessor),IUserService
    {
        public async Task<UserProfileDto> GetUserProfileAsync()
        {
            var user = await GetCurrentUserAsync();

            var userTrips = await _unitOfWork.Set<Trip>()
                .Include(t => t.TripPlaces)
                    .ThenInclude(tp => tp.Place)          // 👈 include Place
                        .ThenInclude(p => p.Images)        // 👈 include Place's images
                .Where(t => t.UserId == user.Id && !t.IsTemplate)
                .OrderByDescending(t => t.StartDate)
                .ToListAsync();

            var completedTrips = userTrips.Count(t => t.Status == TripStatusOption.Completed);

            var visitedPlaces = userTrips
                .Where(t => t.Status == TripStatusOption.Completed)
                .SelectMany(t => t.TripPlaces)
                .Select(tp => tp.PlaceId)
                .Distinct()
                .Count();

            var savedPlaces = await _unitOfWork.Set<UserPlaces>()
                .CountAsync(up => up.UserId == user.Id);

            var recentTrips = userTrips
                .Take(2)  // 👈 changed from 5 to 2
                .Select(t => new RecentTripDto
                {
                    Id = t.Id,
                    TripName = t.TripName,
                    StartDate = t.StartDate,
                    MainImageUrl = t.TripPlaces
                        .FirstOrDefault()               // first place in the trip
                        ?.Place?.Images
                        .FirstOrDefault(img => img.IsMain)  // its main image
                        ?.ImageUrl                          // the URL
                })
                .ToList();

            return new UserProfileDto
            {
                UserName = user.UserName,
                Email = user.Email,
                CompletedTripsCount = completedTrips,
                VisitedPlacesCount = visitedPlaces,
                SavedPlacesCount = savedPlaces,
                RecentTrips = recentTrips
            };
        }
       
        public async Task<AdminProfileDto> GetDashboardStatsAsync()
        {
            var user = await GetCurrentUserAsync();
            var totalUsers = await _userManager.Users.CountAsync();

            // ✅ 2. Total places
            var totalPlaces = await _unitOfWork.Set<Places>().CountAsync();

            // ✅ 3. Total trips
            var totalTrips = await _unitOfWork.Set<Trip>().CountAsync();

            // ✅ 4. Total scans (if you have a Scan entity)
            var scanCounter = await _unitOfWork.Set<Scan>().FirstOrDefaultAsync();
            var totalScans = scanCounter?.TotalScans ?? 0;

            // 5. Return DTO
            return new AdminProfileDto
            {
                UserName = user.UserName,
                Email = user.Email,
                TotalUsersCount = totalUsers,
                TotalPlacesCount = totalPlaces,
                TotalTripPlansCount = totalTrips,
                TotalScansCount = totalScans
            };
        }

     
    }

    
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.TripAndPlaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Shared.DTOs.PlacesDto;

namespace Services
{
    public class BaseService(UserManager<Users> _userManager, IHttpContextAccessor _httpContextAccessor)
    {
        protected async Task<int> GetCurrentUserIdAsync()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User not authenticated");
            return int.Parse(userIdClaim);
        }

        // Helper to get current user entity
        protected async Task<Users> GetCurrentUserAsync()
        {
            var userId = await GetCurrentUserIdAsync();
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new UnauthorizedAccessException("User not found");
            return user;
        }

    }
}

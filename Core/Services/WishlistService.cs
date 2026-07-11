using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Abstraction;
using Shared.DTOs.PlacesDto;

namespace Services
{
    public class WishlistService(IUnitOfWork _unitOfWork, IMapper _mapper, UserManager<Users> _userManager, IHttpContextAccessor _httpContextAccessor) : 
        BaseService(_userManager, _httpContextAccessor), IWishlistService
    {
       
        public async Task<IEnumerable<PlacesResultDTO>> GetUserWishlistAsync()
        {
            var userId = await GetCurrentUserIdAsync();
            // Get all UserPlaces for this user, including the related Place with its Category and Images
            var userPlaces = await _unitOfWork.Set<UserPlaces>()
                .Where(up => up.UserId == userId)
                .Include(up => up.Place)
                    .ThenInclude(p => p.Category)
                .Include(up => up.Place.Images)
                .ToListAsync();

            // Extract the places
            var places = userPlaces.Select(up => up.Place).ToList();

            // Map to DTO (AutoMapper will handle CategoryName, etc.)
            return _mapper.Map<IEnumerable<PlacesResultDTO>>(places);
        }

        public async Task AddToWishlistAsync(int placeId)
        {
            var userId = await GetCurrentUserIdAsync();
            // Check if already exists
            var exists = await _unitOfWork.Set<UserPlaces>()
                .AnyAsync(up => up.UserId == userId && up.PlaceId == placeId);
            if (exists)
                return; // or throw a custom exception if you prefer

            var userPlace = new UserPlaces
            {
                UserId = userId,
                PlaceId = placeId
                // You may set CreatedAt if you have such a property
            };

            await _unitOfWork.Set<UserPlaces>().AddAsync(userPlace);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveFromWishlistAsync(int placeId)
        {
            var userId = await GetCurrentUserIdAsync();
            var userPlace = await _unitOfWork.Set<UserPlaces>()
                .FirstOrDefaultAsync(up => up.UserId == userId && up.PlaceId == placeId);
            if (userPlace != null)
            {
                _unitOfWork.Set<UserPlaces>().Remove(userPlace);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        
    }

}
     

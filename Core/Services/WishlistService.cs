using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Contracts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Services.Abstraction;
using Shared.DTOs;

namespace Services
{
    public class WishlistService(IUnitOfWork _unitOfWork, IMapper _mapper) : IWishlistService
    {

        public async Task<IEnumerable<PlacesResultDTO>> GetUserWishlistAsync(int userId)
        {
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

        public async Task AddToWishlistAsync(int userId, int placeId)
        {
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

        public async Task RemoveFromWishlistAsync(int userId, int placeId)
        {
            var userPlace = await _unitOfWork.Set<UserPlaces>()
                .FirstOrDefaultAsync(up => up.UserId == userId && up.PlaceId == placeId);
            if (userPlace != null)
            {
                _unitOfWork.Set<UserPlaces>().Remove(userPlace);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> IsInWishlistAsync(int userId, int placeId)
        {
            return await _unitOfWork.Set<UserPlaces>()
                .AnyAsync(up => up.UserId == userId && up.PlaceId == placeId);
        }
    }

}
     

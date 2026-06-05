using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DTOs;

namespace Services.Abstraction
{
    public interface IWishlistService
    {
        Task<IEnumerable<PlacesResultDTO>> GetUserWishlistAsync(int userId);
        Task AddToWishlistAsync(int userId, int placeId);
        Task RemoveFromWishlistAsync(int userId, int placeId);
        Task<bool> IsInWishlistAsync(int userId, int placeId);
    }
}

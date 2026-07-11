using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DTOs.PlacesDto;

namespace Services.Abstraction
{
    public interface IWishlistService
    {
        Task<IEnumerable<PlacesResultDTO>> GetUserWishlistAsync();
        Task AddToWishlistAsync(int placeId);
        Task RemoveFromWishlistAsync(int placeId);
        
    }
}

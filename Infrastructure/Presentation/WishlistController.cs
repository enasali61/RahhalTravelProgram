using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction;
using Shared.DTOs.PlacesDto;

namespace Presentation
{
    public class WishlistController(IWishlistService _wishlistService) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlacesResultDTO>>> GetWishlist()
        {
            // TODO: get userId from JWT            
            var items = await _wishlistService.GetUserWishlistAsync();
            return Ok(items);
        }

        [HttpPost("{placeId}")]
        public async Task<IActionResult> AddToWishlist(int placeId)
        {
            await _wishlistService.AddToWishlistAsync( placeId);
            return NoContent();
        }

        [HttpDelete("{placeId}")]
        public async Task<IActionResult> RemoveFromWishlist(int placeId)
        {
            await _wishlistService.RemoveFromWishlistAsync(placeId);
            return NoContent();
        }

        
    }
}

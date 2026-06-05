using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction;
using Shared.DTOs;

namespace Presentation
{
    public class WishlistController(IWishlistService _wishlistService) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlacesResultDTO>>> GetWishlist()
        {
            // TODO: get userId from JWT
            int userId = 1; // placeholder
            var items = await _wishlistService.GetUserWishlistAsync(userId);
            return Ok(items);
        }

        [HttpPost("{placeId}")]
        public async Task<IActionResult> AddToWishlist(int placeId)
        {
            int userId = 1; // placeholder
            await _wishlistService.AddToWishlistAsync(userId, placeId);
            return NoContent();
        }

        [HttpDelete("{placeId}")]
        public async Task<IActionResult> RemoveFromWishlist(int placeId)
        {
            int userId = 1; // placeholder
            await _wishlistService.RemoveFromWishlistAsync(userId, placeId);
            return NoContent();
        }

        [HttpGet("check/{placeId}")]
        public async Task<ActionResult<bool>> IsInWishlist(int placeId)
        {
            int userId = 1; // placeholder
            var exists = await _wishlistService.IsInWishlistAsync(userId, placeId);
            return Ok(exists);
        }
    }
}

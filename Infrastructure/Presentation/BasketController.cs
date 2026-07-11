using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction;
using Shared.DTOs;

namespace Presentation
{
    [Authorize]
    public class BasketController(IServiceManager serviceManager) : BaseApiController
    {

        [HttpGet]
        public async Task<ActionResult<BasketDto>> GetBasket([FromQuery]string? language = "en", [FromQuery] int travelersCount = 1)
        {
            var basket = await serviceManager.BasketService.GetBasketAsync(language, travelersCount);
            return Ok(basket);
        }

        [HttpPost("items")]
        public async Task<ActionResult<BasketDto>> AddItem([FromBody] AddToBasketDto itemDto, [FromQuery] string? language = "en", [FromQuery] int travelersCount = 1)
        {
            var basket = await serviceManager.BasketService.AddItemAsync(itemDto,language,travelersCount);
            return Ok(basket);
        }

        [HttpDelete("items/{itemId}")]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            await serviceManager.BasketService.RemoveItemAsync(itemId);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> ClearBasket()
        {
            await serviceManager.BasketService.ClearBasketAsync();
            return NoContent();
        }
    }
}

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
        [HttpGet("{id}")]
        public async Task<ActionResult<BasketDto>> GetBasket(string id)
        {
            var basket = await serviceManager.BasketService.GetBasketAsync(id);
            return Ok( basket);
        }
        [HttpPost]
        public async Task<ActionResult<BasketDto>> UpdateBasket(BasketDto basketDto)
        {
            var basket = await serviceManager.BasketService.CreateOrUpdateBasketAsync(basketDto);
            return Ok(basket);
        }
        [HttpDelete("{id}")]

        public async Task<ActionResult> DeleteBasket(string id)
        {
           await serviceManager.BasketService.DeleteBasketAsync(id);
            return NoContent();
        }
    }
}

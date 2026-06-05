using Shared.DTOs;

namespace Services.Abstraction
{
    public interface IBasketService
    {
        public Task<BasketDto> GetBasketAsync(string basketId);
        public Task<BasketDto> CreateOrUpdateBasketAsync(BasketDto basket);
        public Task<bool> DeleteBasketAsync(string basketId);

    }
}

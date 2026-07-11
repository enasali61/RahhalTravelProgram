using Shared.DTOs;

namespace Services.Abstraction
{
    public interface IBasketService
    {
        public Task<BasketDto> GetBasketAsync(string? language, int travelersCount);
        Task<BasketDto> AddItemAsync( AddToBasketDto itemDto,string? language, int travelersCount);
        Task<BasketDto?> UpdateBasketAsync(BasketDto basket);
        public Task ClearBasketAsync();
        public Task RemoveItemAsync(int itemId);

    }
}

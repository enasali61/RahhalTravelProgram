using Domain.Entities;

namespace Domain.Contracts
{
    public interface IBasketRepository
    {
        // get basket
        public Task<CustomerBasket?> GetBasketAsync(string basketId);
        // delete basket
        public Task<bool> DeleteBasketAsync(string basketId);
        // create or update basket
        public Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket, TimeSpan? timeToLive = null);
    }
}

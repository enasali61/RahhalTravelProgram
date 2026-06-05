using System.Text.Json;
using Domain.Contracts;
using Domain.Entities;
using StackExchange.Redis;
namespace Presistence.Repositories
{
    public class BasketRepository(IConnectionMultiplexer connectionMultiplexer) : IBasketRepository
    {
        private readonly IDatabase _database = connectionMultiplexer.GetDatabase();
        public async Task<bool> DeleteBasketAsync(string basketId)
        => await _database.KeyDeleteAsync(basketId);

        public async Task<CustomerBasket?> GetBasketAsync(string basketId)
        {
            var value = await _database.StringGetAsync(basketId); // json
            if (value.IsNullOrEmpty)
                return null;
            return JsonSerializer.Deserialize<CustomerBasket>(value); // to c# object
        }

        public async Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket, TimeSpan? timeToLive = null)
        {
            var JsonBasket = JsonSerializer.Serialize(basket); // to json in memory
            var created = await _database.StringSetAsync(basket.Id, JsonBasket, timeToLive ?? TimeSpan.FromDays(30));
            return created ? await GetBasketAsync(basket.Id) : null;
        }  
    }
}

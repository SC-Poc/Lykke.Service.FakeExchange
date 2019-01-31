using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.FakeExchange.Domain.Services
{
    public interface IFakeExchange
    {
        Task<string> GetNameAsync();
        
        Task<Guid> CreateOrderAsync(Order order);
        
        Task<IReadOnlyCollection<Order>> GetOrdersAsync(string clientId);
        
        Task<IReadOnlyDictionary<string, decimal>> GetBalancesAsync(string clientId);
        
        Task SetBalanceAsync(string clientId, string asset, decimal balance);

        Task CancelLimitOrderAsync(string clientId, Guid orderId);

        Task<IReadOnlyCollection<string>> GetAllInstrumentsAsync();

        Task<OrderBook> GetOrderBookAsync(string assetPair);

        Task PublishOrderBooksAsync();

        Task HandleExternalOrderBookAsync(
            string source,
            string assetPairId,
            IReadOnlyCollection<Order> buyOrders,
            IReadOnlyCollection<Order> sellOrders);
    }
}


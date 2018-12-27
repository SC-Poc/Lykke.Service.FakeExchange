using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.FakeExchange.Domain.Services
{
    public interface IFakeApiClient
    {
        Task<Guid> CreateLimitOrderAsync(string pair, decimal price, decimal volume, TradeType tradeType);

        Task<IReadOnlyCollection<Order>> GetOrdersAsync();
        
        Task<IReadOnlyDictionary<string, decimal>> GetBalancesAsync();
        
        Task SetBalanceAsync(string asset, decimal balance);
        
        Task CancelLimitOrderAsync(Guid orderId);
        
        Task<Guid> CreateMarketOrderAsync(string pair, TradeType tradeType, decimal volume);
    }
}

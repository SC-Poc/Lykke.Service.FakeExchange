using System;
using System.Collections.Generic;

namespace Lykke.Service.FakeExchange.Domain.Services
{
    public interface IFakeApiClient
    {
        Guid CreateLimitOrder(string pair, decimal price, decimal volume, TradeType tradeType);

        IEnumerable<Order> GetOrders();
        
        IDictionary<string, decimal> GetBalances();
        
        void SetBalance(string asset, decimal balance);
        
        void CancelLimitOrder(Guid orderId);
        
        Guid  CreateMarketOrder(string pair, TradeType tradeType, decimal volume);
    }
}

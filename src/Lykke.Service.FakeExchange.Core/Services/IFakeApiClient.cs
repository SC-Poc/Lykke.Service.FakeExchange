using System;
using System.Collections.Generic;
using Lykke.Service.FakeExchange.Core.Domain;

namespace Lykke.Service.FakeExchange.Core.Services
{
    public interface IFakeApiClient
    {
        Guid CreateLimitOrder(string pair, decimal price, decimal volume, TradeType tradeType);

        IEnumerable<Order> GetLimitOrders();
        
        IDictionary<string, decimal> GetBalances();
        
        void SetBalance(string asset, decimal balance);
        
        void CancelLimitOrder(Guid orderId);
    }
}

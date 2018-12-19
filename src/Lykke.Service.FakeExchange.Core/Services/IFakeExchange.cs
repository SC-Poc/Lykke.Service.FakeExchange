using System;
using System.Collections.Generic;
using Lykke.Service.FakeExchange.Core.Domain;

namespace Lykke.Service.FakeExchange.Core.Services
{
    public interface IFakeExchange
    {
        Guid CreateOrder(Order order);
        
        IEnumerable<Order> GetOrders(string clientId);
        
        IDictionary<string, decimal> GetBalances(string clientId);
        
        void SetBalance(string clientId, string asset, decimal balance);

        void CancelLimitOrder(string clientId, Guid orderId);
        
        IEnumerable<string> GetAllInstruments();
        
        OrderBook GetOrderBook(string assetPair);

        void PublishOrderBooksAsync();

        void HandleExternalOrderBook(
            string source,
            string assetPairId,
            IReadOnlyCollection<Order> buyOrders,
            IReadOnlyCollection<Order> sellOrders);
    }
}


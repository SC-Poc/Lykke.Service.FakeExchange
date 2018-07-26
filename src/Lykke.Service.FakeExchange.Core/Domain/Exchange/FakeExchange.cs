using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Lykke.Service.FakeExchange.Core.Services;

namespace Lykke.Service.FakeExchange.Core.Domain.Exchange
{
    public class FakeExchange : IFakeExchange
    {
        public static string Name = "fakeExchange";
        
        private readonly IBalancesService _balancesService;
        private readonly ConcurrentDictionary<string, OrderBook> _orderBooks = 
            new ConcurrentDictionary<string, OrderBook>(StringComparer.InvariantCultureIgnoreCase);
        
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<Guid, Order>> _userOrders = 
            new ConcurrentDictionary<string, ConcurrentDictionary<Guid, Order>>();

        public event Action<OrderBook> OrderBookChanged;
        
        public FakeExchange(IBalancesService balancesService)
        {
            _balancesService = balancesService;
        }
        
        public Guid CreateOrder(Order order)
        {
            _userOrders.TryAdd(order.ClientId, new ConcurrentDictionary<Guid, Order>());
            _userOrders[order.ClientId].TryAdd(order.Id, order);

            if (_orderBooks.TryAdd(order.Pair, new OrderBook(order.Pair, _balancesService)))
            {
                _orderBooks[order.Pair].OrderBookChanged += ob => OrderBookChanged?.Invoke(ob);
            }
            _orderBooks[order.Pair].Add(order);

            return order.Id;
        }
        
        public IEnumerable<Order> GetOrders(string clientId)
        {
            if (_userOrders.TryGetValue(clientId, out var ordersDictionary))
            {
                return ordersDictionary.Values;
            }

            return Enumerable.Empty<Order>();
        }

        public IDictionary<string, decimal> GetBalances(string clientId)
        {
            return _balancesService.GetBalances(clientId);
        }

        public void SetBalance(string clientId, string asset, decimal balance)
        {
            _balancesService.SetBalance(clientId, asset, balance);
        }

        public void CancelLimitOrder(string clientId, Guid orderId)
        {
            if (_userOrders.TryGetValue(clientId, out var userOrders) && 
                userOrders.TryGetValue(orderId, out var order) &&
                _orderBooks.TryGetValue(order.Pair, out var orderBook))
            {
                orderBook.Cancel(order);
            }
        }
    }
}

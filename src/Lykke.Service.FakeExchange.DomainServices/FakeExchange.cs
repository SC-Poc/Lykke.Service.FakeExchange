using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Lykke.Service.FakeExchange.Domain;
using Lykke.Service.FakeExchange.Domain.Services;

namespace Lykke.Service.FakeExchange.DomainServices
{
    public class FakeExchange : IFakeExchange
    {
        public static string Name = "fakeExchange";

        private readonly bool _matchExternalOrderBooks;
        private readonly IBalancesService _balancesService;
        private readonly IOrderBookPublisher _orderBookPublisher;
        private readonly ITickPricePublisher _tickPricePublisher;

        private readonly ConcurrentDictionary<string, OrderBook> _orderBooks = 
            new ConcurrentDictionary<string, OrderBook>(StringComparer.InvariantCultureIgnoreCase);
        
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<Guid, Order>> _userOrders = 
            new ConcurrentDictionary<string, ConcurrentDictionary<Guid, Order>>();

        public FakeExchange(
            IBalancesService balancesService,
            IOrderBookPublisher orderBookPublisher,
            ITickPricePublisher tickPricePublisher,
            bool matchExternalOrderBooks)
        {
            _balancesService = balancesService;
            _orderBookPublisher = orderBookPublisher;
            _tickPricePublisher = tickPricePublisher;
            _matchExternalOrderBooks = matchExternalOrderBooks;
        }

        public IEnumerable<string> GetAllInstruments()
        {
            return _orderBooks.Keys;
        }

        public OrderBook GetOrderBook(string assetPair)
        {
            _orderBooks.TryGetValue(assetPair, out var orderBook);

            return orderBook;
        }
        
        public Guid CreateOrder(Order order)
        {
            Guid orderId = AddOrder(order);

            PublishOrderBookAsync(order.Pair);

            return orderId;
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

                PublishOrderBookAsync(order.Pair);
            }
        }

        public void HandleExternalOrderBook(
            string source,
            string assetPairId,
            IReadOnlyCollection<Order> buyOrders,
            IReadOnlyCollection<Order> sellOrders)
        {
            if (!_matchExternalOrderBooks)
                return;

            OrderBook orderBook = GetOrAddOrderBook(assetPairId);
            orderBook.Remove(o => o.IsExternal);

            foreach (var order in buyOrders)
            {
                AddOrder(order);
            }

            foreach (var order in sellOrders)
            {
                AddOrder(order);
            }

            PublishOrderBookAsync(assetPairId);
        }

        private Guid AddOrder(Order order)
        {
            _userOrders.TryAdd(order.ClientId, new ConcurrentDictionary<Guid, Order>());
            _userOrders[order.ClientId].TryAdd(order.Id, order);

            OrderBook orderBook = GetOrAddOrderBook(order.Pair);
            orderBook.Add(order);

            return order.Id;
        }

        private OrderBook GetOrAddOrderBook(string assetPairId)
        {
            OrderBook orderBook = _orderBooks.GetOrAdd(assetPairId, new OrderBook(assetPairId, _balancesService));
            return orderBook;
        }

        public void PublishOrderBooksAsync()
        {
            foreach (var orderBook in _orderBooks.Values)
            {
                if (!orderBook.IsEmpty)
                {
                    _orderBookPublisher.PublishAsync(orderBook).GetAwaiter().GetResult();

                    _tickPricePublisher.PublishAsync(TickPrice.FromOrderBook(FakeExchange.Name, orderBook)).GetAwaiter().GetResult();
                }
            }
        }

        private void PublishOrderBookAsync(string assetPairId)
        {
            OrderBook orderBook = GetOrAddOrderBook(assetPairId);

            if (!orderBook.IsEmpty)
            {
                _orderBookPublisher.PublishAsync(orderBook).GetAwaiter().GetResult();

                _tickPricePublisher.PublishAsync(TickPrice.FromOrderBook(FakeExchange.Name, orderBook)).GetAwaiter().GetResult();
            }
        }
    }
}

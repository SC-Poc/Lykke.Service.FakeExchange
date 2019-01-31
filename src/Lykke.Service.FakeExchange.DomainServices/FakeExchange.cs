using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.FakeExchange.Domain;
using Lykke.Service.FakeExchange.Domain.Services;

namespace Lykke.Service.FakeExchange.DomainServices
{
    public class FakeExchange : IFakeExchange
    {
        private readonly bool _matchExternalOrderBooks;
        private readonly IBalancesService _balancesService;
        private readonly IClientOrdersService _clientOrdersService;
        private readonly IOrderBookPublisher _orderBookPublisher;
        private readonly ITickPricePublisher _tickPricePublisher;
        
        private readonly string _exchangeName;
        
        private readonly ConcurrentDictionary<string, OrderBook> _orderBooks = 
            new ConcurrentDictionary<string, OrderBook>(StringComparer.InvariantCultureIgnoreCase);
        
        public FakeExchange(
            IBalancesService balancesService,
            IClientOrdersService clientOrdersService,
            IOrderBookPublisher orderBookPublisher,
            ITickPricePublisher tickPricePublisher,
            bool matchExternalOrderBooks,
            string exchangeName)
        {
            _balancesService = balancesService;
            _clientOrdersService = clientOrdersService;
            _orderBookPublisher = orderBookPublisher;
            _tickPricePublisher = tickPricePublisher;
            _matchExternalOrderBooks = matchExternalOrderBooks;
            _exchangeName = exchangeName;
        }

        public Task<IReadOnlyCollection<string>> GetAllInstrumentsAsync()
        {
            return Task.FromResult((IReadOnlyCollection<string>)_orderBooks.Keys.ToArray());
        }

        public Task<OrderBook> GetOrderBookAsync(string assetPair)
        {
            _orderBooks.TryGetValue(assetPair, out OrderBook orderBook);

            return Task.FromResult(orderBook);
        }
        
        public Task<string> GetNameAsync()
        {
            return Task.FromResult(_exchangeName);
        }
        
        public async Task<Guid> CreateOrderAsync(Order order)
        {
            _clientOrdersService.Add(order);

            OrderBook orderBook = _orderBooks.GetOrAdd(order.Pair, new OrderBook(order.Pair, _balancesService));

            orderBook.Add(order);

            await PublishOrderBookAsync(orderBook);

            return order.Id;
        }

        public Task<IReadOnlyCollection<Order>> GetOrdersAsync(string clientId)
        {
            return Task.FromResult(_clientOrdersService.GetByClientId(clientId));
        }

        public Task<IReadOnlyDictionary<string, decimal>> GetBalancesAsync(string clientId)
        {
            return Task.FromResult(_balancesService.GetBalances(clientId));
        }

        public Task SetBalanceAsync(string clientId, string asset, decimal balance)
        {
            _balancesService.SetBalance(clientId, asset, balance);

            return Task.CompletedTask;
        }

        public async Task CancelLimitOrderAsync(string clientId, Guid orderId)
        {
            Order order = _clientOrdersService.GetById(clientId, orderId);

            if (_orderBooks.TryGetValue(order.Pair, out OrderBook orderBook))
            {
                orderBook.Cancel(order);

                await PublishOrderBookAsync(orderBook);
            }
        }

        public Task HandleExternalOrderBookAsync(
            string source,
            string assetPairId,
            IReadOnlyCollection<Order> buyOrders,
            IReadOnlyCollection<Order> sellOrders)
        {
            if (!_matchExternalOrderBooks)
                return Task.CompletedTask;

            OrderBook orderBook = _orderBooks.GetOrAdd(assetPairId, new OrderBook(assetPairId, _balancesService));

            orderBook.Remove(o => o.IsExternal);

            foreach (Order order in buyOrders.OrderByDescending(o => o.Price))
            {
                orderBook.Add(order);
            }

            foreach (Order order in sellOrders.OrderBy(o => o.Price))
            {
                orderBook.Add(order);
            }

            return PublishOrderBookAsync(orderBook);
        }

        public async Task PublishOrderBooksAsync()
        {
            foreach (OrderBook orderBook in _orderBooks.Values)
            {
                await PublishOrderBookAsync(orderBook);
            }
        }

        private async Task PublishOrderBookAsync(OrderBook orderBook)
        {
            if (!orderBook.IsEmpty)
            {
                await _orderBookPublisher.PublishAsync(orderBook);

                await _tickPricePublisher.PublishAsync(TickPrice.FromOrderBook(_exchangeName, orderBook));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.Service.FakeExchange.Core.Domain.Exceptions;
using Lykke.Service.FakeExchange.Core.Services;

namespace Lykke.Service.FakeExchange.Core.Domain
{
    public class OrderBook
    {
        private readonly IBalancesService _balancesService;

        private readonly object _sync = new object();
        
        private readonly List<Order> _buySide = new List<Order>();

        private readonly List<Order> _sellSide = new List<Order>();

        public string Pair { get; }

        public IReadOnlyList<Order> Asks
        {
            get
            {
                lock (_sync)
                {
                    return _sellSide.ToList();
                }
            }
        }

        public IReadOnlyList<Order> Bids
        {
            get
            {
                lock (_sync)
                {
                    return _buySide.ToList();
                }
            }
        }

        public IReadOnlyList<Order> AllOrders
        {
            get
            {
                lock (_sync)
                {
                    return _buySide.Union(_sellSide).ToList();
                }
            }
        }
        
        public event Action<OrderBook> OrderBookChanged;

        public OrderBook(
            string pair,
            IBalancesService balancesService)
        {
            Pair = pair;
            _balancesService = balancesService;
        }
        
        public void Add(Order order)
        {
            lock (_sync)
            {
                Validate(order);

                TryExecute(order);

                if (order.HasRemainingVolume)
                {
                    AddToOrderBook(order);
                }

                RemoveExecutedOrders();
            }
            
            OrderBookChanged?.Invoke(this);
        }

        private void RemoveExecutedOrders()
        {
            _sellSide.Where(x => !x.HasRemainingVolume).ToList().ForEach(x => _sellSide.Remove(x));
            _buySide.Where(x => !x.HasRemainingVolume).ToList().ForEach(x => _buySide.Remove(x));
        }

        private void AddToOrderBook(Order order)
        {
            if (order.TradeType == TradeType.Buy)
            {
                _buySide.Add(order);
            }

            if (order.TradeType == TradeType.Sell)
            {
                _sellSide.Add(order);
            }
        }

        private void Validate(Order order)
        {
            if (!string.Equals(order.Pair, Pair, StringComparison.InvariantCultureIgnoreCase))
            {
                order.Reject();
                throw new InvalidInstrumentException($"OrderBook for {Pair} can't accept orders for {order.Pair}");
            }
            
            if (!_balancesService.UserHasEnoughBalanceForOrder(order))
            {
                order.Reject();
                throw new InsufficientBalanceException($"User {order.ClientId} can't place order {order}");
            }
        }

        private bool TryExecute(Order order)
        {
            if (order.OrderType == OrderType.Limit)
            {
                return TryExecuteLimit(order);
            }

            if (order.OrderType == OrderType.Market)
            {
                return TryExecuteMarket(order);
            }

            return false;
        }

        private bool TryExecuteLimit(Order order)
        {
            var ordersForMatching = order.TradeType == TradeType.Buy
                    ? _sellSide.Where(x => x.Price <= order.Price)
                    : _buySide.Where(x => x.Price >= order.Price);
                    
            MatchOrders(order, ordersForMatching);

            return order.HasExecutions;
        }

        private void MatchOrders(Order order, IEnumerable<Order> ordersForMatching)
        {
            var sortedOrdersForMatching = order.TradeType == TradeType.Buy
                ? ordersForMatching.OrderBy(x => x.Price)
                : ordersForMatching.OrderByDescending(x => x.Price); 
            
            foreach (var orderForMatching in sortedOrdersForMatching)
            {
                var volumeForExecution = Math.Min(orderForMatching.RemainingVolume, order.RemainingVolume);

                if (volumeForExecution > 0)
                {
                    orderForMatching.Execute(volumeForExecution, orderForMatching.Price);
                    order.Execute(volumeForExecution, orderForMatching.Price);


                    var sellerId = new[] {order, orderForMatching}.Single(x => x.TradeType == TradeType.Sell).ClientId;
                    var buyerId = new[] {order, orderForMatching}.Single(x => x.TradeType == TradeType.Buy).ClientId;

                    _balancesService.ExchangeBalancesDueToExecution(sellerId, buyerId, order.Pair, volumeForExecution,
                        orderForMatching.Price);
                }

                if (!order.HasRemainingVolume)
                {
                    break;
                }
            }
        }

        private bool TryExecuteMarket(Order order)
        {
            var ordersForMatching = (order.TradeType == TradeType.Buy ? _sellSide : _buySide).ToList();

            if (ordersForMatching.Sum(x => x.RemainingVolume) < order.RemainingVolume)
            {
                order.Reject();
                throw new NotEnoughLiquidityException($"Not enough liquidity to execute market order {order}");
            }

            MatchOrders(order, ordersForMatching);
            
            return order.HasExecutions;
        }

        public void Cancel(Order order)
        {
            lock (_sync)
            {
                if (order.TradeType == TradeType.Sell)
                {
                    if (_sellSide.Remove(order))
                    {
                        order.Cancel();
                        OrderBookChanged?.Invoke(this);
                    }
                }
                else if (order.TradeType == TradeType.Buy)
                {
                    if (_buySide.Remove(order))
                    {
                        order.Cancel();
                        OrderBookChanged?.Invoke(this);
                    }
                }
            }
        }
    }
}

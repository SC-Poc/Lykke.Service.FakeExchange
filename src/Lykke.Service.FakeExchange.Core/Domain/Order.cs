using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Lykke.Service.FakeExchange.Core.Domain
{
    public class Order
    {
        private Order(string clientId, OrderType orderType, TradeType tradeType, string pair, decimal price, decimal volume, bool isExternal = false)
        {
            Id = Guid.NewGuid();
            CreationDateTime = DateTime.UtcNow;
            
            ClientId = clientId;
            OrderType = orderType;
            TradeType = tradeType;
            Pair = pair;
            Price = price;
            Volume = volume;
            IsExternal = isExternal;
            OrderStatus = OrderStatus.Active;
        }
        
        public Guid Id { get; }
        
        public DateTime CreationDateTime { get; }
        
        public string ClientId { get; }
        
        public OrderType OrderType { get; }
        
        public decimal Price { get; }
        
        public decimal Volume { get; }
        
        public TradeType TradeType { get; }
        
        public string Pair { get; }
        
        public OrderStatus OrderStatus { get; private set; }

        public bool IsExternal { get; }

        public decimal ExecutedVolume => _executions.Sum(x => x.Item1);

        public decimal AvgExecutionPrice => HasExecutions ? _executions.Sum(x => x.Item1 * x.Item2) / _executions.Sum(x => x.Item1) : 0;

        public bool HasRemainingVolume => RemainingVolume > 0;

        public decimal RemainingVolume => Volume - ExecutedVolume;

        private readonly List<Tuple<decimal, decimal>> _executions = new List<Tuple<decimal, decimal>>();

        public bool HasExecutions => _executions.Any();

        internal void Execute(decimal volume, decimal price)
        {
            if (OrderStatus == OrderStatus.Canceled || OrderStatus == OrderStatus.Fill ||
                OrderStatus == OrderStatus.Rejected)
            {
                throw new InvalidOperationException($"Can't execute order in status {OrderStatus}. Order: {this}.");
            }
            
            _executions.Add(Tuple.Create(volume, price));

            if (!HasRemainingVolume)
            {
                OrderStatus = OrderStatus.Fill;    
            }
        }

        internal void Cancel()
        {
            if (OrderStatus == OrderStatus.Active)
            {
                OrderStatus = OrderStatus.Canceled;
            }
        }

        internal void Reject()
        {
            OrderStatus = OrderStatus.Rejected;
        }

        public override string ToString()
        {
            return this.ToJson();
        }
        
        public static Order CreateLimit(string clientId, TradeType tradeType, string pair, decimal price,
            decimal volume, bool isExternal = false)
        {
            return new Order(clientId, OrderType.Limit, tradeType, pair, price, volume, isExternal);
        }

        public static Order CreateMarket(string clientId, TradeType tradeType, string pair, decimal volume,
            bool isExternal = false)
        {
            return new Order(clientId, OrderType.Market, tradeType, pair, Decimal.Zero, volume, isExternal);
        }
    }
}

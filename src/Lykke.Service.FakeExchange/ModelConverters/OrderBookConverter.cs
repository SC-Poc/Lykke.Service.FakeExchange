using System;
using System.Linq;
using Lykke.Common.ExchangeAdapter.Contracts;

namespace Lykke.Service.FakeExchange.ModelConverters
{
    public static class OrderBookConverter
    {
        public static OrderBook ToModel(this Core.Domain.OrderBook orderBook)
        {
            return new OrderBook(Core.Domain.FakeExchange.Name,
                orderBook.Pair,
                DateTime.UtcNow,
                orderBook.Asks.Select(x => new OrderBookItem(x.Price, x.RemainingVolume)),
                orderBook.Bids.Select(x => new OrderBookItem(x.Price, x.RemainingVolume)));
        }
    }
}

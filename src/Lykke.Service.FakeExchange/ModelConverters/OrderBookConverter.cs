using System;
using System.Linq;
using Lykke.Common.ExchangeAdapter.Contracts;

namespace Lykke.Service.FakeExchange.ModelConverters
{
    public static class OrderBookConverter
    {
        public static OrderBook ToModel(this Domain.OrderBook orderBook, string exchangeName)
        {
            return new OrderBook(exchangeName,
                orderBook.Pair,
                DateTime.UtcNow,
                orderBook.Asks.Select(x => new OrderBookItem(x.Price, x.RemainingVolume)),
                orderBook.Bids.Select(x => new OrderBookItem(x.Price, x.RemainingVolume)));
        }
    }
}

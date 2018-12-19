using System;

namespace Lykke.Service.FakeExchange.Domain
{
    public class TickPrice
    {
        public string Source { get; set; }

        public string AssetPair { get; set; }

        public DateTime Timestamp { get; set; }

        public decimal Ask { get; set; }

        public decimal Bid { get; set; }

        public static TickPrice FromOrderBook(string source, OrderBook orderBook)
        {
            return new TickPrice
            {
                Source = source,
                AssetPair = orderBook.Pair,
                Timestamp = DateTime.UtcNow,
                Ask = orderBook.BestAskPrice,
                Bid = orderBook.BestBidPrice
            };
        }
    }
}

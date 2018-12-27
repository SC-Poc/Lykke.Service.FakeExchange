using System;
using Lykke.Service.FakeExchange.Domain;

namespace Lykke.Service.FakeExchange.ModelConverters
{
    public static class TradeTypeConverter
    {
        public static TradeType ToDomainTradeType(this Common.ExchangeAdapter.Contracts.TradeType tradeType)
        {
            switch (tradeType)
            {
                case Common.ExchangeAdapter.Contracts.TradeType.Buy:
                    return TradeType.Buy;
                case Common.ExchangeAdapter.Contracts.TradeType.Sell:
                    return TradeType.Sell;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tradeType), tradeType, null);
            }
        }
    }
}

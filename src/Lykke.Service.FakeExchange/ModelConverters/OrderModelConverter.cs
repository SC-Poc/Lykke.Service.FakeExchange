using System;
using Lykke.Common.ExchangeAdapter.SpotController.Records;
using Lykke.Service.FakeExchange.Domain;
using OrderStatus = Lykke.Common.ExchangeAdapter.SpotController.Records.OrderStatus;
using TradeType = Lykke.Common.ExchangeAdapter.Contracts.TradeType;

namespace Lykke.Service.FakeExchange.ModelConverters
{
    public static class OrderModelConverter
    {
        public static OrderModel ToModel(this Order order)
        {
            return new OrderModel
            {
                Symbol = order.Pair,
                Id = order.Id.ToString(),
                Price = order.Price,
                OriginalVolume = order.Volume,
                ExecutionStatus = ConvertStatus(order.OrderStatus),
                AvgExecutionPrice = order.AvgExecutionPrice,
                ExecutedVolume = order.ExecutedVolume,
                RemainingAmount = order.RemainingVolume,
                TradeType = ConvertTradeType(order.TradeType),
                Timestamp = order.CreationDateTime
            };
        }

        private static TradeType ConvertTradeType(Domain.TradeType orderTradeType)
        {
            switch (orderTradeType)
            {
                case Domain.TradeType.Buy:
                    return TradeType.Buy;
                case Domain.TradeType.Sell:
                    return TradeType.Sell;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orderTradeType), orderTradeType, null);
            }
        }

        private static OrderStatus ConvertStatus(Domain.OrderStatus orderStatus)
        {
            switch (orderStatus)
            {
                case Domain.OrderStatus.Active:
                    return OrderStatus.Active;
                
                case Domain.OrderStatus.Fill:
                    return OrderStatus.Fill;
                
                case Domain.OrderStatus.Canceled:
                case Domain.OrderStatus.Rejected:
                    return OrderStatus.Canceled;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(orderStatus), orderStatus, null);
            }
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter.Server;
using Lykke.Common.ExchangeAdapter.SpotController.Records;
using Lykke.Service.FakeExchange.Domain.Services;
using Lykke.Service.FakeExchange.ModelConverters;
using Microsoft.AspNetCore.Mvc;
using OrderStatus = Lykke.Service.FakeExchange.Domain.OrderStatus;

namespace Lykke.Service.FakeExchange.Controllers
{
    public class SpotController : SpotControllerBase<IFakeApiClient>
    {
        public override Task<OrderIdResponse> CreateLimitOrderAsync([FromBody] LimitOrderRequest request)
        {
            return Task.FromResult(new OrderIdResponse
            {
                OrderId = Api.CreateLimitOrder(request.Instrument, request.Price, request.Volume, request.TradeType.ToDomainTradeType()).ToString()
            });
        }
        
        public override Task<GetLimitOrdersResponse> GetLimitOrdersAsync()
        {
            return Task.FromResult(new GetLimitOrdersResponse
            {
                Orders = Api.GetOrders().Where(x => x.OrderStatus == OrderStatus.Active).Select(x => x.ToModel()).ToList()
            });
        }

        public override Task<GetOrdersHistoryResponse> GetOrdersHistoryAsync()
        {
            return Task.FromResult(new GetOrdersHistoryResponse
            {
                Orders = Api.GetOrders().Where(x => x.OrderStatus != OrderStatus.Active).Select(x => x.ToModel()).ToList()
            });
        }

        public override Task<CancelLimitOrderResponse> CancelLimitOrderAsync([FromBody] CancelLimitOrderRequest request)
        {
            if (Guid.TryParse(request.OrderId, out var orderId))
            {
                Api.CancelLimitOrder(orderId);
            }

            return Task.FromResult(new CancelLimitOrderResponse
                {
                    OrderId = request.OrderId
                });
        }

        public override Task<OrderModel> LimitOrderStatusAsync(string orderId)
        {
            if (Guid.TryParse(orderId, out var id))
            {
                return Task.FromResult(Api.GetOrders().SingleOrDefault(x => x.Id == id)?.ToModel());
            }

            return Task.FromResult((OrderModel)null);
        }

        public override Task<GetWalletsResponse> GetWalletBalancesAsync()
        {
            return Task.FromResult(new GetWalletsResponse()
            {
                Wallets = Api.GetBalances().Select(x => new WalletBalanceModel()
                {
                    Asset = x.Key,
                    Balance = x.Value
                }).ToList()
            });
        }

        public override Task<OrderIdResponse> CreateMarketOrderAsync([FromBody] MarketOrderRequest request)
        {
            return Task.FromResult(new OrderIdResponse
            {
                OrderId = Api.CreateMarketOrder(request.Instrument, request.TradeType.ToDomainTradeType(), request.Volume).ToString()
            });
        }

        public override Task<OrderModel> MarketOrderStatusAsync(string orderId)
        {
            if (Guid.TryParse(orderId, out var id))
            {
                return Task.FromResult(Api.GetOrders().SingleOrDefault(x => x.Id == id)?.ToModel());
            }

            return Task.FromResult((OrderModel)null);
        }

        public override Task<OrderIdResponse> ReplaceLimitOrderAsync([FromBody] ReplaceLimitOrderRequest request)
        {
            if (Guid.TryParse(request.OrderIdToCancel, out var orderIdToCancel))
            {
                Api.CancelLimitOrder(orderIdToCancel);
            }
            
            return Task.FromResult(new OrderIdResponse
            {
                OrderId = Api.CreateLimitOrder(request.Instrument, request.Price, request.Volume, request.TradeType.ToDomainTradeType()).ToString()
            });
        }
    }
}

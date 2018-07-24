using System;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter.Server;
using Lykke.Common.ExchangeAdapter.SpotController.Records;
using Lykke.Service.FakeExchange.Core.Services;
using Lykke.Service.FakeExchange.ModelConverters;
using Microsoft.AspNetCore.Mvc;
using OrderStatus = Lykke.Service.FakeExchange.Core.Domain.OrderStatus;
using TradeType = Lykke.Common.ExchangeAdapter.Contracts.TradeType;

namespace Lykke.Service.FakeExchange.Controllers
{
    public class SpotController : SpotControllerBase<IFakeApiClient>
    {
        public override Task<OrderIdResponse> CreateLimitOrderAsync([FromBody] LimitOrderRequest request)
        {
            var id = Api.CreateLimitOrder(request.Instrument, request.Price, request.Volume,
                request.TradeType == TradeType.Buy ? Core.Domain.TradeType.Buy : Core.Domain.TradeType.Sell);

            return Task.FromResult(new OrderIdResponse()
            {
                OrderId = id.ToString()
            });
        }

        public override Task<GetLimitOrdersResponse> GetLimitOrdersAsync()
        {
            return Task.FromResult(new GetLimitOrdersResponse
            {
                Orders = Api.GetLimitOrders().Where(x => x.OrderStatus == OrderStatus.Active).Select(x => x.ToModel()).ToList()
            });
        }

        public override Task<GetOrdersHistoryResponse> GetOrdersHistoryAsync()
        {
            return Task.FromResult(new GetOrdersHistoryResponse
            {
                Orders = Api.GetLimitOrders().Where(x => x.OrderStatus != OrderStatus.Active).Select(x => x.ToModel()).ToList()
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
                return Task.FromResult(Api.GetLimitOrders().SingleOrDefault(x => x.Id == id)?.ToModel());
            }

            return Task.FromResult((OrderModel)null);
        }
    }
}

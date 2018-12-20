using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter.Server;
using Lykke.Common.ExchangeAdapter.SpotController.Records;
using Lykke.Service.FakeExchange.Domain;
using Lykke.Service.FakeExchange.Domain.Services;
using Lykke.Service.FakeExchange.ModelConverters;
using Microsoft.AspNetCore.Mvc;
using OrderStatus = Lykke.Service.FakeExchange.Domain.OrderStatus;

namespace Lykke.Service.FakeExchange.Controllers
{
    public class SpotController : SpotControllerBase<IFakeApiClient>
    {
        public override async Task<OrderIdResponse> CreateLimitOrderAsync([FromBody] LimitOrderRequest request)
        {
            Guid orderId = await Api.CreateLimitOrderAsync(
                request.Instrument, request.Price, request.Volume, request.TradeType.ToDomainTradeType());

            return new OrderIdResponse
            {
                OrderId = orderId.ToString()
            };
        }
        
        public override async Task<GetLimitOrdersResponse> GetLimitOrdersAsync()
        {
            IEnumerable<Order> orders = (await Api.GetOrdersAsync());

            IReadOnlyCollection<OrderModel> model = orders
                .Where(x => x.OrderStatus == OrderStatus.Active)
                .Select(x => x.ToModel())
                .ToArray();

            return new GetLimitOrdersResponse
            {
                Orders = model
            };
        }

        public override async Task<GetOrdersHistoryResponse> GetOrdersHistoryAsync()
        {
            IEnumerable<Order> orders = await Api.GetOrdersAsync();

            IReadOnlyCollection<OrderModel> model = orders
                .Where(x => x.OrderStatus != OrderStatus.Active)
                .Select(x => x.ToModel())
                .ToArray();

            return new GetOrdersHistoryResponse
            {
                Orders = model
            };
        }

        public override async Task<CancelLimitOrderResponse> CancelLimitOrderAsync([FromBody] CancelLimitOrderRequest request)
        {
            if (Guid.TryParse(request.OrderId, out Guid orderId))
            {
                await Api.CancelLimitOrderAsync(orderId);
            }

            return new CancelLimitOrderResponse
            {
                OrderId = request.OrderId
            };
        }

        public override async Task<OrderModel> LimitOrderStatusAsync(string orderId)
        {
            if (Guid.TryParse(orderId, out Guid id))
            {
                IEnumerable<Order> orders = await Api.GetOrdersAsync();

                return orders.SingleOrDefault(x => x.Id == id)?.ToModel();
            }

            return null;
        }

        public override async Task<GetWalletsResponse> GetWalletBalancesAsync()
        {
            var balances = await Api.GetBalancesAsync();

            return new GetWalletsResponse
            {
                Wallets = balances.Select(x => new WalletBalanceModel
                {
                    Asset = x.Key,
                    Balance = x.Value
                }).ToList()
            };
        }

        public override async Task<OrderIdResponse> CreateMarketOrderAsync([FromBody] MarketOrderRequest request)
        {
            Guid orderId = await Api.CreateMarketOrderAsync(
                request.Instrument, request.TradeType.ToDomainTradeType(), request.Volume);

            return new OrderIdResponse
            {
                OrderId = orderId.ToString()
            };
        }

        public override async Task<OrderModel> MarketOrderStatusAsync(string orderId)
        {
            if (Guid.TryParse(orderId, out Guid id))
            {
                IEnumerable<Order> orders = await Api.GetOrdersAsync();

                return orders.SingleOrDefault(x => x.Id == id)?.ToModel();
            }

            return null;
        }

        public override async Task<OrderIdResponse> ReplaceLimitOrderAsync([FromBody] ReplaceLimitOrderRequest request)
        {
            if (Guid.TryParse(request.OrderIdToCancel, out Guid orderIdToCancel))
            {
                await Api.CancelLimitOrderAsync(orderIdToCancel);
            }

            Guid orderId = await Api.CreateLimitOrderAsync(
                request.Instrument, request.Price, request.Volume, request.TradeType.ToDomainTradeType());

            return new OrderIdResponse
            {
                OrderId = orderId.ToString()
            };
        }
    }
}

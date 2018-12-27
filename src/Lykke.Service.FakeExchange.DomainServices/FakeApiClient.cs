using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.FakeExchange.Domain;
using Lykke.Service.FakeExchange.Domain.Services;

namespace Lykke.Service.FakeExchange.DomainServices
{
    public class FakeApiClient : IFakeApiClient
    {
        private readonly string _clientId;
        private readonly IFakeExchange _fakeExchange;

        public FakeApiClient(string clientId, IFakeExchange fakeExchange)
        {
            _clientId = clientId;
            _fakeExchange = fakeExchange;
        }

        public Task<Guid> CreateLimitOrderAsync(string pair, decimal price, decimal volume, TradeType tradeType)
        {
            return _fakeExchange.CreateOrderAsync(Order.CreateLimit(_clientId, tradeType, pair, price, volume));
        }

        public Task<IReadOnlyCollection<Order>> GetOrdersAsync()
        {
            return _fakeExchange.GetOrdersAsync(_clientId);
        }

        public Task<IReadOnlyDictionary<string, decimal>> GetBalancesAsync()
        {
            return _fakeExchange.GetBalancesAsync(_clientId);
        }

        public Task SetBalanceAsync(string asset, decimal balance)
        {
            return _fakeExchange.SetBalanceAsync(_clientId, asset, balance);
        }

        public Task CancelLimitOrderAsync(Guid orderId)
        {
            return _fakeExchange.CancelLimitOrderAsync(_clientId, orderId);
        }

        public Task<Guid> CreateMarketOrderAsync(string pair, TradeType tradeType, decimal volume)
        {
            return _fakeExchange.CreateOrderAsync(Order.CreateMarket(_clientId, tradeType, pair, volume));
        }
    }
}

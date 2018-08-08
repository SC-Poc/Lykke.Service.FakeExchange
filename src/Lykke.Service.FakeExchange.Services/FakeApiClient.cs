using System;
using System.Collections.Generic;
using Lykke.Service.FakeExchange.Core.Domain;
using Lykke.Service.FakeExchange.Core.Services;

namespace Lykke.Service.FakeExchange.Services
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

        public Guid CreateLimitOrder(string pair, decimal price, decimal volume, TradeType tradeType)
        {
            return _fakeExchange.CreateOrder(Order.CreateLimit(_clientId, tradeType, pair, price, volume));
        }

        public IEnumerable<Order> GetOrders()
        {
            return _fakeExchange.GetOrders(_clientId);
        }

        public IDictionary<string, decimal> GetBalances()
        {
            return _fakeExchange.GetBalances(_clientId);
        }

        public void SetBalance(string asset, decimal balance)
        {
            _fakeExchange.SetBalance(_clientId, asset, balance);
        }

        public void CancelLimitOrder(Guid orderId)
        {
            _fakeExchange.CancelLimitOrder(_clientId, orderId);
        }

        public Guid CreateMarketOrder(string pair, TradeType tradeType, decimal volume)
        {
            return _fakeExchange.CreateOrder(Order.CreateMarket(_clientId, tradeType, pair, volume));
        }
    }
}

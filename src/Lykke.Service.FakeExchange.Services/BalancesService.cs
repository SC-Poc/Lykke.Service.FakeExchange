using System.Collections.Concurrent;
using System.Collections.Generic;
using Lykke.Service.FakeExchange.Core.Domain;
using Lykke.Service.FakeExchange.Core.Services;

namespace Lykke.Service.FakeExchange.Services
{
    public class BalancesService : IBalancesService
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, decimal>> _balances = new ConcurrentDictionary<string, ConcurrentDictionary<string, decimal>>();
        
        public IDictionary<string, decimal> GetBalances(string clientId)
        {
            if (_balances.TryGetValue(clientId, out var clientBalances))
            {
                return clientBalances;
            }
            else
            {
                return new Dictionary<string, decimal>();
            }
        }

        public void SetBalance(string clientId, string asset, decimal balance)
        {
            _balances.AddOrUpdate(clientId,
                new ConcurrentDictionary<string, decimal>(new Dictionary<string, decimal>
                {
                    {asset, balance}
                }),
                (key, prevValue) =>
                    {
                        prevValue.AddOrUpdate(asset, balance, (assetKey, prevBalance) => balance);
                        return prevValue;
                    });
        }

        private bool CanBuy(string clientId, string pair, decimal volume)
        {
            return true; // TODO: split pair to base and quote assets, test balance against quote asset
        }

        private bool CanSell(string clientId, string pair, decimal volume)
        {
            return true; // TODO: split pair to base and quote assets, test balance against base asset
        }

        public bool UserHasEnoughBalanceForOrder(Order order)
        {
            return order.TradeType == TradeType.Buy && CanBuy(order.ClientId, order.Pair, order.Volume * order.Price)
                   ||
                   order.TradeType == TradeType.Sell && CanSell(order.ClientId, order.Pair, order.Volume);
        }
    }
}

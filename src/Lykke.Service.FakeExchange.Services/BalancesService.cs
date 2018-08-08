using System;
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
                new ConcurrentDictionary<string, decimal>(
                    new Dictionary<string, decimal>
                        {
                            {asset, balance}
                        },
                    StringComparer.InvariantCultureIgnoreCase),
                (key, prevValue) =>
                    {
                        prevValue.AddOrUpdate(asset, balance, (assetKey, prevBalance) => balance);
                        return prevValue;
                    });
        }

        public bool UserHasEnoughBalanceForOrder(Order order)
        {
            return order.TradeType == TradeType.Buy && CanBuy(order.ClientId, order.Pair, order.Volume * order.Price)
                   ||
                   order.TradeType == TradeType.Sell && CanSell(order.ClientId, order.Pair, order.Volume);
        }
        
        private bool CanBuy(string clientId, string pair, decimal volume)
        {
            if (TryGetAssetsFromPair(pair, out _, out var quoteAsset) && TryGetBalance(clientId, quoteAsset, out var balance))
            {
                return balance >= volume;
            }
            
            return true;
        }

        private bool CanSell(string clientId, string pair, decimal volume)
        {
            if (TryGetAssetsFromPair(pair, out var baseAsset, out _) && TryGetBalance(clientId, baseAsset, out var balance))
            {
                return balance >= volume;
            }
            
            return true;
        }
        
        private bool TryGetBalance(string clientId, string asset, out decimal balance)
        {
            var balances = GetBalances(clientId);
            if (balances.ContainsKey(asset))
            {
                balance = balances[asset];
                return true;
            }

            balance = default(decimal);
            return false;
        }

        private bool TryGetAssetsFromPair(string pair, out string baseAsset, out string quoteAsset)
        {
            if (pair.Length == 6)
            {
                baseAsset = pair.Substring(0, 3);
                quoteAsset = pair.Substring(3, 3);
                return true;
            }

            baseAsset = null;
            quoteAsset = null;
            return false;
        }

        public void ExchangeBalancesDueToExecution(
            string sellerId, 
            string buyerId,
            string pair,
            decimal executionVolume,
            decimal executionPrice)
        {
            if (!TryGetAssetsFromPair(pair, out var baseAsset, out var quoteAsset))
            {
                return;
            }
            
            decimal baseAmount = executionVolume;
            decimal quoteAmount = executionVolume * executionPrice;
            
            AddBalance(sellerId, baseAsset, -baseAmount);
            AddBalance(buyerId, baseAsset, baseAmount);
            
            AddBalance(sellerId, quoteAsset, quoteAmount);
            AddBalance(buyerId, quoteAsset, -quoteAmount);
        }
        
        private void AddBalance(string clientId, string asset, decimal balance)
        {
            if (_balances.TryGetValue(clientId, out var userBalances))
            {
                userBalances.AddOrUpdate(asset, balance, (key, prevValue) => prevValue + balance);
            }
        }
    }
}

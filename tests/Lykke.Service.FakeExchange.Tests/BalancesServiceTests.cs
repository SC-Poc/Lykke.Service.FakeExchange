using Lykke.Service.FakeExchange.Core.Domain;
using Lykke.Service.FakeExchange.Services;
using Xunit;

namespace Lykke.Service.FakeExchange.Tests
{
    public class BalancesServiceTests
    {
        private const string ClientId1 = "1";
        private const string Pair = "btcusd";
        private const string BaseAsset = "btc";
        private const string QuoteAsset = "usd";
        
        [Fact]
        public void BalanceIsNotSet_ItsEnought()
        {
            var balancesService = new BalancesService();

            var order = Order.CreateLimit(ClientId1, TradeType.Sell, Pair, 100, 100);

            bool enoughtFundsForOrder = balancesService.UserHasEnoughBalanceForOrder(order);
            
            Assert.True(enoughtFundsForOrder);
        }
        
        [Fact]
        public void SellOrder_BaseAssetBalanceIsSetHigh_ItsEnought()
        {
            var balancesService = new BalancesService();
            balancesService.SetBalance(ClientId1, BaseAsset, 1000);

            var order = Order.CreateLimit(ClientId1, TradeType.Sell, Pair, 100, 100);

            bool enoughtFundsForOrder = balancesService.UserHasEnoughBalanceForOrder(order);
            
            Assert.True(enoughtFundsForOrder);
        }
        
        [Fact]
        public void SellOrder_BaseAssetBalanceIsSetLow_ItsNotEnought()
        {
            var balancesService = new BalancesService();
            balancesService.SetBalance(ClientId1, BaseAsset, 10);

            var order = Order.CreateLimit(ClientId1, TradeType.Sell, Pair, 100, 100);

            bool enoughtFundsForOrder = balancesService.UserHasEnoughBalanceForOrder(order);
            
            Assert.False(enoughtFundsForOrder);
        }
        
        [Fact]
        public void BuyOrder_QuoteBalanceIsSetHigh_ItsEnought()
        {
            var balancesService = new BalancesService();
            balancesService.SetBalance(ClientId1, QuoteAsset, 100*100);

            var order = Order.CreateLimit(ClientId1, TradeType.Buy, Pair, 100, 100);

            bool enoughtFundsForOrder = balancesService.UserHasEnoughBalanceForOrder(order);
            
            Assert.True(enoughtFundsForOrder);
        }
        
        [Fact]
        public void BuyOrder_QuoteBalanceIsSetLow_ItsNotEnought()
        {
            var balancesService = new BalancesService();
            balancesService.SetBalance(ClientId1, QuoteAsset, 1000);

            var order = Order.CreateLimit(ClientId1, TradeType.Buy, Pair, 100, 100);

            bool enoughtFundsForOrder = balancesService.UserHasEnoughBalanceForOrder(order);
            
            Assert.False(enoughtFundsForOrder);
        }
    }
}

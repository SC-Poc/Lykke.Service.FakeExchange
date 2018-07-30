using System.Linq;
using Lykke.Service.FakeExchange.Core.Domain;
using Lykke.Service.FakeExchange.Core.Domain.Exceptions;
using Lykke.Service.FakeExchange.Core.Services;
using Moq;
using Xunit;

namespace Lykke.Service.FakeExchange.Tests
{
    public class OrderBookLimitOrdersTests
    {
        private const string ClientId1 = "1";
        private const string ClientId2 = "2";
        private const string Pair = "btcusd";
        
        [Fact]
        public void TwoOrdersMatchedCompletely()
        {
            var ob = CreateOrderBook();

            var sell = Order.CreateLimit(ClientId1, TradeType.Sell, Pair, 100, 100);
            var buy = Order.CreateLimit(ClientId2, TradeType.Buy, Pair, 100, 100);
            
            ob.Add(buy);
            
            Assert.Equal(1, ob.AllOrders.Count());
            
            ob.Add(sell);
            
            Assert.True(sell.HasExecutions);
            Assert.False(sell.HasRemainingVolume);
            
            Assert.True(buy.HasExecutions);
            Assert.False(buy.HasRemainingVolume);
            
            Assert.False(ob.AllOrders.Any());
        }

        private static OrderBook CreateOrderBook()
        {
            var balancesService = new Mock<IBalancesService>();
            balancesService.Setup(x => x.UserHasEnoughBalanceForOrder(It.IsAny<Order>())).Returns(true);

            var ob = new OrderBook(Pair, balancesService.Object);
            return ob;
        }

        [Fact]
        public void TwoOrdersStaysInOrderBook()
        {
            var ob = CreateOrderBook();

            var sell = Order.CreateLimit(ClientId1, TradeType.Sell, Pair, 101, 100);
            var buy = Order.CreateLimit(ClientId2, TradeType.Buy, Pair, 99, 100);
            
            ob.Add(buy);
            
            Assert.Equal(1, ob.AllOrders.Count());
            
            ob.Add(sell);
            
            Assert.Equal(2, ob.AllOrders.Count());
            
            Assert.False(buy.HasExecutions);
            Assert.False(sell.HasExecutions);
        }

        [Fact]
        public void BuyOrderHasLargerVolume()
        {
            var ob = CreateOrderBook();

            var sell = Order.CreateLimit(ClientId1, TradeType.Sell, Pair, 100, 100);
            var buy = Order.CreateLimit(ClientId2, TradeType.Buy, Pair, 100, 60);
            
            ob.Add(buy);
            
            Assert.Equal(1, ob.AllOrders.Count());
            
            ob.Add(sell);
            
            Assert.Equal(1, ob.AllOrders.Count());
            Assert.Equal(sell, ob.AllOrders.Single());
            
            Assert.True(buy.HasExecutions);
            Assert.False(buy.HasRemainingVolume);
            
            Assert.True(sell.HasExecutions);
            Assert.Equal(40, sell.RemainingVolume);
        }

        [Fact]
        public void BuyOrderHasSmallerVolume()
        {
            var ob = CreateOrderBook();

            var sell = Order.CreateLimit(ClientId1, TradeType.Sell, Pair, 100, 60);
            var buy = Order.CreateLimit(ClientId2, TradeType.Buy, Pair, 100, 100);
            
            ob.Add(buy);
            
            Assert.Equal(1, ob.AllOrders.Count());
            
            ob.Add(sell);
            
            Assert.Equal(1, ob.AllOrders.Count());
            Assert.Equal(buy, ob.AllOrders.Single());
            
            Assert.True(buy.HasExecutions);
            Assert.True(buy.HasRemainingVolume);
            
            Assert.True(sell.HasExecutions);
            Assert.False(sell.HasRemainingVolume);
        }

        [Fact]
        public void ClientHasInsufficientFunds()
        {
            var balancesService = new Mock<IBalancesService>();
            balancesService.Setup(x => x.UserHasEnoughBalanceForOrder(It.IsAny<Order>())).Returns(false);

            var ob = new OrderBook(Pair, balancesService.Object);
            
            var sell = Order.CreateLimit(ClientId1, TradeType.Sell, Pair, 100, 60);

            Assert.Throws<InsufficientBalanceException>(() => ob.Add(sell));
        }
    }
}

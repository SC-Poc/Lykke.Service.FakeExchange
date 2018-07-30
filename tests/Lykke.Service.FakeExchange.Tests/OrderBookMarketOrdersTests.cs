using Lykke.Service.FakeExchange.Core.Domain;
using Lykke.Service.FakeExchange.Core.Domain.Exceptions;
using Lykke.Service.FakeExchange.Core.Services;
using Moq;
using Xunit;

namespace Lykke.Service.FakeExchange.Tests
{
    public class OrderBookMarketOrdersTests
    {
        private const string ClientId1 = "1";
        private const string ClientId2 = "2";
        private const string Pair = "btcusd";

        [Fact]
        public void EmptyOrderBook_NotEnoughLiquidity()
        {
            var ob = CreateOrderBook();

            var market = Order.CreateMarket(ClientId1, TradeType.Buy, Pair, 100);

            Assert.Throws<NotEnoughLiquidityException>(() => ob.Add(market));
        }

        [Fact]
        public void OneOrderHasSmallVolume_NotEnoughLiquidity()
        {
            var ob = CreateOrderBook();

            var sell = Order.CreateLimit(ClientId1, TradeType.Sell, Pair, 100, 100);
            var market = Order.CreateMarket(ClientId2, TradeType.Buy, Pair, 150);
            
            ob.Add(sell);
            
            Assert.Throws<NotEnoughLiquidityException>(() => ob.Add(market));
        }
        
        [Fact]
        public void OneOrderHasEqualVolume_Match()
        {
            var ob = CreateOrderBook();

            var sell = Order.CreateLimit(ClientId1, TradeType.Sell, Pair, 100, 100);
            var market = Order.CreateMarket(ClientId2, TradeType.Buy, Pair, 100);
            
            ob.Add(sell);
            ob.Add(market);
            
            Assert.Equal(OrderStatus.Fill, sell.OrderStatus);
            Assert.Equal(OrderStatus.Fill, market.OrderStatus);
        }

        [Fact]
        public void TwoOrdersHasEqualVolume_MatchWithBoth()
        {
            var ob = CreateOrderBook();
            
            var sell1 = Order.CreateLimit(ClientId1, TradeType.Sell, Pair, 100, 60);
            var sell2 = Order.CreateLimit(ClientId1, TradeType.Sell, Pair, 150, 40);
            var market = Order.CreateMarket(ClientId2, TradeType.Buy, Pair, 100);
            
            ob.Add(sell1);
            ob.Add(sell2);
            ob.Add(market);
            
            Assert.Equal(OrderStatus.Fill, sell1.OrderStatus);
            Assert.Equal(OrderStatus.Fill, sell2.OrderStatus);
            Assert.Equal(OrderStatus.Fill, market.OrderStatus);
            
            Assert.Equal((60m * 100m + 40m * 150m)/100m, market.AvgExecutionPrice);
        }

        [Fact]
        public void TwoOrdersHasMoreVolume_MatchWithLowestPriceFirst()
        {
            var ob = CreateOrderBook();
            
            var sell1 = Order.CreateLimit(ClientId1, TradeType.Sell, Pair, 100, 60);
            var sell2 = Order.CreateLimit(ClientId1, TradeType.Sell, Pair, 150, 60);
            var market = Order.CreateMarket(ClientId2, TradeType.Buy, Pair, 100);
            
            ob.Add(sell1);
            ob.Add(sell2);
            ob.Add(market);
            
            Assert.Equal(OrderStatus.Fill, market.OrderStatus);
            Assert.Equal(OrderStatus.Fill, sell1.OrderStatus);
            Assert.Equal(OrderStatus.Active, sell2.OrderStatus);
            
            Assert.Equal(20, sell2.RemainingVolume);
            
        }

        private static OrderBook CreateOrderBook()
        {
            var balancesService = new Mock<IBalancesService>();
            balancesService.Setup(x => x.UserHasEnoughBalanceForOrder(It.IsAny<Order>())).Returns(true);

            var ob = new OrderBook(Pair, balancesService.Object);
            return ob;
        }
    }
}

using Lykke.Common.ExchangeAdapter.Server;
using Lykke.Service.FakeExchange.RabbitMq;

namespace Lykke.Service.FakeExchange.Controllers
{
    public class OrderbookController : OrderBookControllerBase
    {
        protected override OrderBooksSession Session { get; }

        public OrderbookController(OrderBookPublisher orderBookPublisher)
        {
            Session = orderBookPublisher.Session;
        }
    }
}

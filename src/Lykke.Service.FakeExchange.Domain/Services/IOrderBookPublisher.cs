using System.Threading.Tasks;

namespace Lykke.Service.FakeExchange.Domain.Services
{
    public interface IOrderBookPublisher
    {
        Task PublishAsync(OrderBook orderBook);
    }
}

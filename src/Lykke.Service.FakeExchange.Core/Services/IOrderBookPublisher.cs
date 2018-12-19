using System.Threading.Tasks;
using Lykke.Service.FakeExchange.Core.Domain;

namespace Lykke.Service.FakeExchange.Core.Services
{
    public interface IOrderBookPublisher
    {
        Task PublishAsync(OrderBook orderBook);
    }
}

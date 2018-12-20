using System.Threading.Tasks;

namespace Lykke.Service.FakeExchange.Domain.Services
{
    public interface ITickPricePublisher
    {
        Task PublishAsync(TickPrice tickPrice);
    }
}

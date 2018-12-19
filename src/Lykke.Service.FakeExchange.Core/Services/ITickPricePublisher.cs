using System.Threading.Tasks;
using Lykke.Service.FakeExchange.Core.Domain;

namespace Lykke.Service.FakeExchange.Core.Services
{
    public interface ITickPricePublisher
    {
        Task PublishAsync(TickPrice tickPrice);
    }
}

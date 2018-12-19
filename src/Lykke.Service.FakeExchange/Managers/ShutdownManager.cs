
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Service.FakeExchange.Services;

namespace Lykke.Service.FakeExchange.Managers
{
    [UsedImplicitly]
    public class ShutdownManager : IShutdownManager
    {
        private readonly OrderBookPublishTimer _orderBookPublishTimer;

        public ShutdownManager(
            OrderBookPublishTimer orderBookPublishTimer)
        {
            _orderBookPublishTimer = orderBookPublishTimer;
        }

        public Task StopAsync()
        {
            _orderBookPublishTimer.Stop();

            return Task.CompletedTask;
        }
    }
}

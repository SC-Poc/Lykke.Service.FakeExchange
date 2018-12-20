using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Service.FakeExchange.DomainServices;

namespace Lykke.Service.FakeExchange.Managers
{
    [UsedImplicitly]
    public class StartupManager : IStartupManager
    {
        private readonly OrderBookPublishTimer _orderBookPublishTimer;

        public StartupManager(
            OrderBookPublishTimer orderBookPublishTimer)
        {
            _orderBookPublishTimer = orderBookPublishTimer;
        }

        public Task StartAsync()
        {
            _orderBookPublishTimer.Start();

            return Task.CompletedTask;
        }
    }
}

using JetBrains.Annotations;
using Lykke.Common.ExchangeAdapter.Server.Settings;

namespace Lykke.Service.FakeExchange.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class FakeExchangeSettings
    {
        public DbSettings Db { get; set; }
        
        public OrderBookProcessingSettings RabbitMq { get; set; }
    }
}

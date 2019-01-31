using JetBrains.Annotations;
using Lykke.Common.ExchangeAdapter.Server.Settings;
using Lykke.Service.FakeExchange.DomainServices.Settings;

namespace Lykke.Service.FakeExchange.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class FakeExchangeSettings
    {
        public string ExchangeName { set; get; }
        
        public DbSettings Db { get; set; }
        
        public OrderBookProcessingSettings RabbitMq { get; set; }

        public ExternalExchangeSettings ExternalExchange { get; set; }

        public MatchingSettings Matching { get; set; }
    }
}

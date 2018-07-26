using JetBrains.Annotations;

namespace Lykke.Service.FakeExchange.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class FakeExchangeSettings
    {
        public DbSettings Db { get; set; }
        
        public RabbitMqSettings RabbitMq { get; set; }
    }
}

using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.FakeExchange.Settings.Rabbit
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class SubscriberSettings
    {
        [AmqpCheck]
        public string ConnectionString { get; set; }

        public string Exchange { get; set; }

        public string Queue { get; set; }
    }
}

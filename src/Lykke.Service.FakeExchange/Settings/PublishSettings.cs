using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.FakeExchange.Settings
{
    public class PublishSettings
    {
        [AmqpCheck]
        public string ConnectionString { get; set; }

        public string Exchanger { get; set; }
    }
}

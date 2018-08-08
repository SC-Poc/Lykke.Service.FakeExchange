using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.FakeExchange.Settings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }
}

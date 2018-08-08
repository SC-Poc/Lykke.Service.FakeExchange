using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.FakeExchange.Settings
{
    [UsedImplicitly]
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnectionString { get; set; }
    }
}

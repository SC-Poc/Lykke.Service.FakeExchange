using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.FakeExchange.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class FakeExchangeSettings
    {
        public DbSettings Db { get; set; }
    }
}

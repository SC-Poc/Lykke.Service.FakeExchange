using JetBrains.Annotations;
using Lykke.Sdk.Settings;

namespace Lykke.Service.FakeExchange.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public FakeExchangeSettings FakeExchangeService { get; set; }        
    }
}

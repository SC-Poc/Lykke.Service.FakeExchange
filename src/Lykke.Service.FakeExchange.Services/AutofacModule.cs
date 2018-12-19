using System;
using Autofac;
using JetBrains.Annotations;
using Lykke.Service.FakeExchange.Core.Services;
using Lykke.Service.FakeExchange.Services.Settings;

namespace Lykke.Service.FakeExchange.Services
{
    [UsedImplicitly]
    public class AutofacModule : Module
    {
        private readonly MatchingSettings _matchingSettings;

        public AutofacModule(
            MatchingSettings matchingSettings)
        {
            _matchingSettings = matchingSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Core.Domain.FakeExchange>()
                .WithParameter(new TypedParameter(typeof(bool), _matchingSettings.MatchExternalOrderBooks))
                .As<IFakeExchange>()
                .SingleInstance();

            builder.RegisterType<BalancesService>()
                .As<IBalancesService>()
                .SingleInstance();

            RegisterTimers(builder);
        }

        private void RegisterTimers(ContainerBuilder builder)
        {
            builder.RegisterType<OrderBookPublishTimer>()
                .WithParameter(new TypedParameter(typeof(TimeSpan), TimeSpan.FromSeconds(5)))
                .AsSelf()
                .SingleInstance();
        }
    }
}

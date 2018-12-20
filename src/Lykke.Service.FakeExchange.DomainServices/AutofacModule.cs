using System;
using Autofac;
using JetBrains.Annotations;
using Lykke.Service.FakeExchange.Domain.Services;
using Lykke.Service.FakeExchange.DomainServices.Balances;
using Lykke.Service.FakeExchange.DomainServices.Settings;

namespace Lykke.Service.FakeExchange.DomainServices
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
            builder.RegisterType<FakeExchange>()
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

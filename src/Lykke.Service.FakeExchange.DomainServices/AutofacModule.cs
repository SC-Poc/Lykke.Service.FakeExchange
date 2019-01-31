using System;
using Autofac;
using JetBrains.Annotations;
using Lykke.Service.FakeExchange.Domain.Services;
using Lykke.Service.FakeExchange.DomainServices.Balances;
using Lykke.Service.FakeExchange.DomainServices.Orders;
using Lykke.Service.FakeExchange.DomainServices.Settings;

namespace Lykke.Service.FakeExchange.DomainServices
{
    [UsedImplicitly]
    public class AutofacModule : Module
    {
        private readonly MatchingSettings _matchingSettings;
        private readonly string _exchangeName;

        public AutofacModule(
            MatchingSettings matchingSettings,
            string exchangeName)
        {
            _matchingSettings = matchingSettings;
            _exchangeName = exchangeName;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FakeExchange>()
                .WithParameter(new TypedParameter(typeof(bool), _matchingSettings.MatchExternalOrderBooks))
                .WithParameter(new TypedParameter(typeof(string), _exchangeName))
                .As<IFakeExchange>()
                .SingleInstance();

            builder.RegisterType<BalancesService>()
                .As<IBalancesService>()
                .SingleInstance();

            builder.RegisterType<ClientOrdersService>()
                .As<IClientOrdersService>()
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

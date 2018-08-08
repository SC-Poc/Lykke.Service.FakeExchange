using Autofac;
using JetBrains.Annotations;
using Lykke.Common.ExchangeAdapter.Server.Settings;
using Lykke.Service.FakeExchange.Core.Services;
using Lykke.Service.FakeExchange.RabbitMq;
using Lykke.Service.FakeExchange.Services;
using Lykke.Service.FakeExchange.Settings;
using Lykke.SettingsReader;
using Microsoft.Extensions.Hosting;

namespace Lykke.Service.FakeExchange.Modules
{
    [UsedImplicitly]
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;

        public ServiceModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Core.Domain.FakeExchange>()
                .As<IFakeExchange>()
                .SingleInstance();

            builder.RegisterType<BalancesService>()
                .As<IBalancesService>()
                .SingleInstance();

            builder.RegisterType<OrderBookPublisher>()
                .As<IHostedService>()
                .AsSelf()
                .WithParameter(new TypedParameter(typeof(OrderBookProcessingSettings),
                    _appSettings.CurrentValue.FakeExchangeService.RabbitMq))
                .SingleInstance();
        }
    }
}

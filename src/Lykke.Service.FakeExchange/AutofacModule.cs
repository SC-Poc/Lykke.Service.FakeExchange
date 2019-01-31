using Autofac;
using JetBrains.Annotations;
using Lykke.Common.ExchangeAdapter.Server.Settings;
using Lykke.Sdk;
using Lykke.Service.FakeExchange.Domain.Services;
using Lykke.Service.FakeExchange.Managers;
using Lykke.Service.FakeExchange.RabbitMq.Publishers;
using Lykke.Service.FakeExchange.RabbitMq.Subscribers;
using Lykke.Service.FakeExchange.Settings;
using Lykke.Service.FakeExchange.Settings.Rabbit;
using Lykke.SettingsReader;
using Microsoft.Extensions.Hosting;

namespace Lykke.Service.FakeExchange
{
    [UsedImplicitly]
    public class AutofacModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;

        public AutofacModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new DomainServices.AutofacModule(
                _settings.CurrentValue.FakeExchangeService.Matching,
                _settings.CurrentValue.FakeExchangeService.ExchangeName));

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            RegisterRabbit(builder);
        }

        private void RegisterRabbit(ContainerBuilder builder)
        {
            builder.RegisterType<OrderBookPublisher>()
                .As<IOrderBookPublisher>()
                .As<IHostedService>()
                .AsSelf()
                .WithParameter(new TypedParameter(typeof(RmqOutput),
                    _settings.CurrentValue.FakeExchangeService.RabbitMq.OrderBooks))
                .SingleInstance();

            builder.RegisterType<TickPricePublisher>()
                .As<ITickPricePublisher>()
                .As<IHostedService>()
                .AsSelf()
                .WithParameter(new TypedParameter(typeof(RmqOutput),
                    _settings.CurrentValue.FakeExchangeService.RabbitMq.TickPrices))
                .SingleInstance();

            builder.RegisterType<ExternalOrderBookSubscriber>()
                .As<IHostedService>()
                .AsSelf()
                .WithParameter(new TypedParameter(typeof(SubscriberSettings),
                    _settings.CurrentValue.FakeExchangeService.ExternalExchange.OrderBooks))
                .SingleInstance();
        }
    }
}

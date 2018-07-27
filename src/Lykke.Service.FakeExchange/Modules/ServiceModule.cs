using Autofac;
using Lykke.Common.ExchangeAdapter.Server.Settings;
using Lykke.Sdk;
using Lykke.Service.FakeExchange.Core.Services;
using Lykke.Service.FakeExchange.RabbitMq;
using Lykke.Service.FakeExchange.Services;
using Lykke.Service.FakeExchange.Settings;
using Lykke.SettingsReader;
using Microsoft.Extensions.Hosting;

namespace Lykke.Service.FakeExchange.Modules
{    
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;

        public ServiceModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
            
            
        }

        protected override void Load(ContainerBuilder builder)
        {
            // Do not register entire settings in container, pass necessary settings to services which requires them

            builder.RegisterType<Core.Domain.Exchange.FakeExchange>()
                .As<IFakeExchange>()
                .SingleInstance();

            builder.RegisterType<BalancesService>()
                .As<IBalancesService>()
                .SingleInstance();

            builder.RegisterType<OrderBookPublisher>()
                .As<IHostedService>()
                .AsSelf()
                .WithParameter(new TypedParameter(typeof(OrderBookProcessingSettings), _appSettings.CurrentValue.FakeExchangeService.RabbitMq))
                .SingleInstance();
        }
    }
}

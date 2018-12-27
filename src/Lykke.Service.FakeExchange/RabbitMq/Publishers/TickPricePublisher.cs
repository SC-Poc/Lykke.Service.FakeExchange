using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Common.ExchangeAdapter.Server.Settings;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.FakeExchange.Domain.Services;
using Microsoft.Extensions.Hosting;

namespace Lykke.Service.FakeExchange.RabbitMq.Publishers
{
    [UsedImplicitly]
    public class TickPricePublisher : ITickPricePublisher, IHostedService, IDisposable
    {
        private readonly RmqOutput _publisherSettings;
        private readonly ILogFactory _logFactory;

        private RabbitMqPublisher<TickPrice> _publisher;

        public TickPricePublisher(RmqOutput publisherSettings, ILogFactory logFactory)
        {
            _publisherSettings = publisherSettings;
            _logFactory = logFactory;
        }

        public Task PublishAsync(Domain.TickPrice tickPrice)
        {
            if (tickPrice.Ask == 0 || tickPrice.Bid == 0)
            {
                return Task.CompletedTask;
            }

            var message = new TickPrice
            {
                Source = tickPrice.Source,
                Asset = tickPrice.AssetPair,
                Timestamp = tickPrice.Timestamp,
                Ask = tickPrice.Ask,
                Bid = tickPrice.Bid
            };

            return _publisher.ProduceAsync(message);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var settings = RabbitMqSubscriptionSettings
                .ForPublisher(_publisherSettings.ConnectionString, _publisherSettings.Exchanger);

            _publisher = new RabbitMqPublisher<TickPrice>(_logFactory, settings)
                .DisableInMemoryQueuePersistence()
                .SetSerializer(new JsonMessageSerializer<TickPrice>())
                .SetPublishStrategy(new DefaultFanoutPublishStrategy(settings))
                .PublishSynchronously()
                .Start();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _publisher?.Stop();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _publisher?.Dispose();
        }
    }
}

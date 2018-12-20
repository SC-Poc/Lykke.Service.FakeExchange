using System;
using System.Linq;
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
    public class OrderBookPublisher : IOrderBookPublisher, IHostedService, IDisposable
    {
        private readonly RmqOutput _publisherSettings;
        private readonly ILogFactory _logFactory;

        private RabbitMqPublisher<OrderBook> _publisher;

        public OrderBookPublisher(RmqOutput publisherSettings, ILogFactory logFactory)
        {
            _publisherSettings = publisherSettings;
            _logFactory = logFactory;
        }

        public Task PublishAsync(Domain.OrderBook orderBook)
        {
            var message = new OrderBook(
                DomainServices.FakeExchange.Name,
                orderBook.Pair,
                DateTime.UtcNow,
                orderBook.Asks.Select(x => new OrderBookItem(x.Price, x.RemainingVolume)),
                orderBook.Bids.Select(x => new OrderBookItem(x.Price, x.RemainingVolume)));

            if (message.AskLevels.IsEmpty && message.BidLevels.IsEmpty)
                return Task.CompletedTask;

            return _publisher.ProduceAsync(message);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var settings = RabbitMqSubscriptionSettings
                .ForPublisher(_publisherSettings.ConnectionString, _publisherSettings.Exchanger);

            _publisher = new RabbitMqPublisher<OrderBook>(_logFactory, settings)
                .DisableInMemoryQueuePersistence()
                .SetSerializer(new JsonMessageSerializer<OrderBook>())
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

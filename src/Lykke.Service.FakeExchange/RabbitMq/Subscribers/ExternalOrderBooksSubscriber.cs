using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.FakeExchange.Core.Domain;
using Lykke.Service.FakeExchange.Core.Services;
using Lykke.Service.FakeExchange.Settings.Rabbit;
using Microsoft.Extensions.Hosting;
using OrderBook = Lykke.Common.ExchangeAdapter.Contracts.OrderBook;

namespace Lykke.Service.FakeExchange.RabbitMq.Subscriber
{
    [UsedImplicitly]
    public class ExternalOrderBookSubscriber : IHostedService, IDisposable
    {
        private readonly SubscriberSettings _settings;
        private readonly IFakeExchange _fakeExchange;
        private readonly ILogFactory _logFactory;
        private readonly ILog _log;

        private RabbitMqSubscriber<OrderBook> _subscriber;

        public ExternalOrderBookSubscriber(
            SubscriberSettings settings,
            IFakeExchange fakeExchange,
            ILogFactory logFactory)
        {
            _settings = settings;
            _fakeExchange = fakeExchange;
            _logFactory = logFactory;
            _log = logFactory.CreateLog(this);
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            var settings = RabbitMqSubscriptionSettings
                .ForSubscriber(_settings.ConnectionString, _settings.Exchange, _settings.Queue);

            settings.DeadLetterExchangeName = null;
            settings.IsDurable = false;

            _subscriber = new RabbitMqSubscriber<OrderBook>(_logFactory, settings,
                    new ResilientErrorHandlingStrategy(_logFactory, settings, TimeSpan.FromSeconds(10)))
                .SetMessageDeserializer(new JsonMessageDeserializer<OrderBook>())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .Subscribe(ProcessMessageAsync)
                .CreateDefaultBinding()
                .Start();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _subscriber?.Stop();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
        }

        private async Task ProcessMessageAsync(OrderBook message)
        {
            try
            {
                var buyOrders = message.BidLevels
                    .Select(o => Order.CreateLimit(message.Source, TradeType.Buy, message.Asset, o.Key, o.Value, true))
                    .ToArray();

                var sellOrders = message.AskLevels
                    .Select(o => Order.CreateLimit(message.Source, TradeType.Sell, message.Asset, o.Key, o.Value, true))
                    .ToArray();

                _fakeExchange.HandleExternalOrderBook(message.Source, message.Asset, buyOrders, sellOrders);
            }
            catch (Exception exception)
            {
                _log.Error(exception, "An error occurred during processing lykke order book", message);
            }
        }
    }
}

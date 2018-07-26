using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Common.ExchangeAdapter.Server;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.FakeExchange.Core.Services;
using Lykke.Service.FakeExchange.Settings;
using Microsoft.Extensions.Hosting;
using ContractOrderBook = Lykke.Common.ExchangeAdapter.Contracts.OrderBook;
using OrderBook = Lykke.Service.FakeExchange.Core.Domain.OrderBook;

namespace Lykke.Service.FakeExchange.RabbitMq
{
    public class OrderBookPublisher : IHostedService
    {
        private readonly ILogFactory _logFactory;
        private readonly IFakeExchange _fakeExchange;
        private readonly ILog _log;
        private readonly RabbitMqSettings _rabbitMqSettings;

        private IDisposable _subscription;
        
        public OrderBookPublisher(ILogFactory logFactory, 
            IFakeExchange fakeExchange,
            RabbitMqSettings rabbitMqSettings)
        {
            _logFactory = logFactory;
            _fakeExchange = fakeExchange;
            _log = logFactory.CreateLog(this);
            _rabbitMqSettings = rabbitMqSettings;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var orderBooksObservable = Observable.Create<ContractOrderBook>(async (obs, ct) =>
            {
                _fakeExchange.OrderBookChanged += book =>
                {
                    obs.OnNext(book.ToContractBook());
                };
                
                var tcs = new TaskCompletionSource<Unit>();
                ct.Register(r => ((TaskCompletionSource<Unit>) r).SetResult(Unit.Default), tcs);
                await tcs.Task;
            });
            
            orderBooksObservable
                //.OnlyWithPositiveSpread()
                .PublishToRmq(
                    _rabbitMqSettings.OrderBooks.ConnectionString,
                    _rabbitMqSettings.OrderBooks.Exchanger, 
                    _logFactory)
                .ReportErrors("OrderBooksPublisher", _log)
                .Publish();

            orderBooksObservable.Select(TickPrice.FromOrderBook)
                .PublishToRmq(
                    _rabbitMqSettings.TickPrices.ConnectionString,
                    _rabbitMqSettings.TickPrices.Exchanger, 
                    _logFactory)
                .ReportErrors("TickPricesPublisher", _log)
                .Publish();

            _subscription = orderBooksObservable.Subscribe();
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _subscription?.Dispose();
            
            return Task.CompletedTask;
        }
    }

    public static class OrderBookConverter
    {
        public static ContractOrderBook ToContractBook(this OrderBook orderBook)
        {
            return new ContractOrderBook(Core.Domain.Exchange.FakeExchange.Name, 
                orderBook.Pair, 
                DateTime.UtcNow,
                orderBook.Asks.Select(x => new OrderBookItem(x.Price, x.Volume)),
                orderBook.Bids.Select(x => new OrderBookItem(x.Price, x.Volume)));
        }
    }
}

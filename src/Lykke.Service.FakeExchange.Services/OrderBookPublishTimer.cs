using System;
using System.Threading.Tasks;
using Common;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.FakeExchange.Core.Services;

namespace Lykke.Service.FakeExchange.Services
{
    [UsedImplicitly]
    public class OrderBookPublishTimer : TimerPeriod
    {
        private readonly IFakeExchange _fakeExchange;

        public OrderBookPublishTimer(
            IFakeExchange fakeExchange,
            TimeSpan publishInterval,
            ILogFactory logFactory) 
            : base(publishInterval, logFactory)
        {
            _fakeExchange = fakeExchange;
        }

        public override Task Execute()
        {
            _fakeExchange.PublishOrderBooksAsync();

            return Task.CompletedTask;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Service.FakeExchange.Core.Services;
using Lykke.Service.FakeExchange.RabbitMq;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.FakeExchange.Controllers
{
    public class OrderbookController : Controller
    {
        private readonly IFakeExchange _fakeExchange;

        public OrderbookController(IFakeExchange fakeExchange)
        {
            _fakeExchange = fakeExchange;
        }
        
        
        [SwaggerOperation("GetAllInstruments")]
        [HttpGet("GetAllInstruments")]
        public IReadOnlyCollection<string> GetAllInstruments()
        {
            return _fakeExchange.GetAllInstruments().ToArray();
        }

        [SwaggerOperation("GetOrderBook")]
        [HttpGet("GetOrderBook")]
        public async Task<OrderBook> GetOrderBook(string assetPair)
        {
            return _fakeExchange.GetOrderBook(assetPair)?.ToContractBook();
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Service.FakeExchange.Domain.Services;
using Lykke.Service.FakeExchange.ModelConverters;
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
        public Task<IReadOnlyCollection<string>> GetAllInstruments()
        {
            return _fakeExchange.GetAllInstrumentsAsync();
        }

        [SwaggerOperation("GetOrderBook")]
        [HttpGet("GetOrderBook")]
        public async Task<OrderBook> GetOrderBook(string assetPair)
        {
            Domain.OrderBook order = await _fakeExchange.GetOrderBookAsync(assetPair);

            return order?.ToModel();
        }
    }
}

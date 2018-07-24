using System.Collections.Generic;
using Lykke.Common.ExchangeAdapter.Server;
using Lykke.Service.FakeExchange.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.FakeExchange.Controllers
{
    [XApiKeyAuth]
    [Route("balance")]
    public class BalanceController : Controller
    {
        private IFakeApiClient RestApi()
        {
            object obj;
            if (HttpContext.Items.TryGetValue((object) "api-credentials", out obj))
                return (IFakeApiClient) obj;
            return null;
        }

        [HttpGet]
        public IDictionary<string, decimal> GetBalances()
        {
            return RestApi().GetBalances();
        }

        [HttpPost]
        public void SetBalance(string asset, decimal balance)
        {
            RestApi().SetBalance(asset, balance);
        }
    }
}

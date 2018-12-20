using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter.Server;
using Lykke.Service.FakeExchange.Domain.Services;
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
        public Task<IReadOnlyDictionary<string, decimal>> GetBalances()
        {
            return RestApi().GetBalancesAsync();
        }

        [HttpPost]
        public Task SetBalance(string asset, decimal balance)
        {
            return RestApi().SetBalanceAsync(asset, balance);
        }
    }
}

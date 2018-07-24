using System.Collections.Generic;
using Lykke.Service.FakeExchange.Core.Domain;

namespace Lykke.Service.FakeExchange.Core.Services
{
    public interface IBalancesService
    {
        IDictionary<string, decimal> GetBalances(string clientId);

        void SetBalance(string clientId, string asset, decimal balance);

        bool UserHasEnoughBalanceForOrder(Order order);
    }
}

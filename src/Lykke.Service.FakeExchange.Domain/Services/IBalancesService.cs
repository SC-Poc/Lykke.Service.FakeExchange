using System.Collections.Generic;

namespace Lykke.Service.FakeExchange.Domain.Services
{
    public interface IBalancesService
    {
        IDictionary<string, decimal> GetBalances(string clientId);

        void SetBalance(string clientId, string asset, decimal balance);

        bool UserHasEnoughBalanceForOrder(Order order);

        void ExchangeBalancesDueToExecution(string sellerId, string buyerId, string pair, decimal executionVolume,
            decimal executionPrice);
    }
}

using System;
using System.Collections.Generic;

namespace Lykke.Service.FakeExchange.Domain.Services
{
    public interface IClientOrdersService
    {
        Order GetById(string clientId, Guid orderId);

        IReadOnlyCollection<Order> GetByClientId(string clientId);

        void Add(Order order);
    }
}

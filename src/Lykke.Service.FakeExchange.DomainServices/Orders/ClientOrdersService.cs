using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Lykke.Service.FakeExchange.Domain;
using Lykke.Service.FakeExchange.Domain.Exceptions;
using Lykke.Service.FakeExchange.Domain.Services;

namespace Lykke.Service.FakeExchange.DomainServices.Orders
{
    public class ClientOrdersService : IClientOrdersService
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<Guid, Order>> _userOrders
            = new ConcurrentDictionary<string, ConcurrentDictionary<Guid, Order>>();

        public IReadOnlyCollection<Order> GetByClientId(string clientId)
        {
            return _userOrders.TryGetValue(clientId, out var ordersDictionary)
                ? ordersDictionary.Values.ToArray()
                : new Order[0];
        }

        public Order GetById(string clientId, Guid orderId)
        {
            if (_userOrders.TryGetValue(clientId, out var userOrders) &&
                userOrders.TryGetValue(orderId, out Order order))
            {
                return order;
            }

            throw new EntityNotFoundException("Order not found.");
        }

        public void Add(Order order)
        {
            _userOrders.TryAdd(order.ClientId, new ConcurrentDictionary<Guid, Order>());

            _userOrders[order.ClientId].TryAdd(order.Id, order);
        }
    }
}

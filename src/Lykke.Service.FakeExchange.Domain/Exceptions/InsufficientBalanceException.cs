using System;

namespace Lykke.Service.FakeExchange.Domain.Exceptions
{
    public class InsufficientBalanceException : Exception
    {
        public InsufficientBalanceException(string message) : base(message)
        {
        }
    }
}

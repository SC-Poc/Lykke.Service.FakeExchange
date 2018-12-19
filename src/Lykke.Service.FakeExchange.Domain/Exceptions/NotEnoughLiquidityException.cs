using System;

namespace Lykke.Service.FakeExchange.Domain.Exceptions
{
    public class NotEnoughLiquidityException : Exception
    {
        public NotEnoughLiquidityException(string message) : base(message)
        {
        }
    }
}

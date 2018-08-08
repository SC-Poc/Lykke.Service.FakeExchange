using System;

namespace Lykke.Service.FakeExchange.Core.Domain.Exceptions
{
    public class NotEnoughLiquidityException : Exception
    {
        public NotEnoughLiquidityException(string message) : base(message)
        {
        }
    }
}

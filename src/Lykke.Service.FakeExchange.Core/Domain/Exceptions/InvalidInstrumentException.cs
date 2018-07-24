using System;

namespace Lykke.Service.FakeExchange.Core.Domain.Exceptions
{
    public class InvalidInstrumentException : Exception
    {
        public InvalidInstrumentException()
        {
        }

        public InvalidInstrumentException(string message) : base(message)
        {
        }
    }
}

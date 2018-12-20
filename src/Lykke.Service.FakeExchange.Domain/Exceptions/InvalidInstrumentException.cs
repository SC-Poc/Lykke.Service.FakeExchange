using System;

namespace Lykke.Service.FakeExchange.Domain.Exceptions
{
    public class InvalidInstrumentException : Exception
    {    
        public InvalidInstrumentException(string message) : base(message)
        {
        }
    }
}

﻿using System;

namespace Lykke.Service.FakeExchange.Core.Domain.Exceptions
{
    public class InsufficientBalanceException : Exception
    {
        public InsufficientBalanceException()
        {
        }

        public InsufficientBalanceException(string message) : base(message)
        {
        }
    }
}
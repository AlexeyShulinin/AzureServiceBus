using System;

namespace AzureServiceBus.Publisher.Api.Exceptions;

public class BaseException : Exception
{
    public BaseException(string message) : base(message) { }
}
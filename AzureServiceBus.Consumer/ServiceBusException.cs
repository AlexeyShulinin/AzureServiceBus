using System;
using Azure.Messaging.ServiceBus;

namespace AzureServiceBus.Consumer;

public class ServiceBusException(ServiceBusReceivedMessage serviceBusReceivedMessage, string message)
    : Exception(message)
{
    public ServiceBusReceivedMessage ServiceBusReceivedMessage { get; init; } = serviceBusReceivedMessage;
}
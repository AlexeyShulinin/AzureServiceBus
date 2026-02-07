using Azure.Messaging.ServiceBus;

namespace AzureServiceBus.Console;

public static class AzureServiceBusClient
{
    public static ServiceBusClient CreateClient(string connectionString)
    {
        var clientOptions = new ServiceBusClientOptions
        {
            TransportType = ServiceBusTransportType.AmqpWebSockets
        };;

        return new ServiceBusClient(connectionString, clientOptions);
    }
}
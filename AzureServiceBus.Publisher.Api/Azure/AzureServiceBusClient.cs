using System.Globalization;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using AzureServiceBus.Publisher.Api.Enums;
using AzureServiceBus.Publisher.Api.Repositories.Models;

namespace AzureServiceBus.Publisher.Api.Azure;

public class AzureServiceBusClient(ServiceBusClient serviceBusClient, JsonSerializerOptions jsonSerializerOptions)
{
    private const string OrdersTopic = "ashul-service-bus-topic";
    
    public async Task SendOrderMessageAsync(OrderDtoModel order, ActionType actionType, CancellationToken cancellationToken)
    {
        var sender = serviceBusClient.CreateSender(OrdersTopic);
        var message = new ServiceBusMessage(JsonSerializer.Serialize(order, jsonSerializerOptions))
        {
            SessionId = order.OrderId,
            ApplicationProperties =
            {
                { "EntityType", "orders" },
                { "ActionType", actionType.ToString() },
                { "Amount", order.Amount.ToString(CultureInfo.InvariantCulture) }
            }
        };
        
        await sender.SendMessageAsync(message, cancellationToken);
    }
}
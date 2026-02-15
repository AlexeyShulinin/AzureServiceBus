using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureServiceBus.Functions;

public class OrdersDeadLetterHandler(ServiceBusClient serviceBusClient, ILogger<OrdersDeadLetterHandler> logger)
{
    private const string TopicName = "ashul-service-bus-topic";
    
    [Function("OrdersDeadLetterHandler")]
    public async Task Run(
        [ServiceBusTrigger(TopicName, "ashul-orders-sub/$deadletterqueue", Connection = "ServiceBusConnectionString")] ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Orders Dead letter Handler Started...");
        
        logger.LogInformation($"Received message: {message.Body}");

        try
        {
            // Return order to topic
            var sender = serviceBusClient.CreateSender(TopicName);
            var retryMessage = new ServiceBusMessage(message.Body)
            {
                SessionId = message.SessionId,
                ApplicationProperties =
                {
                    { "EntityType", "orders" },
                    { "ActionType", "retry" }
                }
            };
            
            await sender.SendMessageAsync(retryMessage, cancellationToken);
            
            await messageActions.CompleteMessageAsync(message, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Orders Dead Letter Handler Failed on message: {message.MessageId}");
        }
    }
}
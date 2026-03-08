using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using AzureServiceBus.Functions.Database;
using AzureServiceBus.Functions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AzureServiceBus.Functions;

public class OrdersDeadLetterHandler(ServiceBusClient serviceBusClient, AppDbContext dbContext, JsonSerializerOptions jsonSerializerOptions, ILogger<OrdersDeadLetterHandler> logger)
{
    private const string TopicName = "ashul-service-bus-topic";
    private const int MaxRetryCount = 2;
    
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
            
            var countOfRetry = message.ApplicationProperties.TryGetValue("CountOfRetry", out var countOfRetryValue) ? Convert.ToInt32(countOfRetryValue) : 0;
            if (countOfRetry >= MaxRetryCount)
            {
                // Ignore message, bcs couldn't be processed
                logger.LogInformation($"Orders Dead Letter Handler Failed on message: {message.MessageId}. Maximum retry count {MaxRetryCount} exceeded.");
                var messageOrder = JsonSerializer.Deserialize<OrderMessageModel>(message.Body, jsonSerializerOptions);
                var dbOrder = await dbContext.Orders.FirstOrDefaultAsync(x => x.OrderId == messageOrder.OrderId, cancellationToken: cancellationToken);
                if (dbOrder != null)
                {
                    logger.LogInformation($"Order {dbOrder.OrderId} has been cancelled.");
                    dbOrder.Status = "Cancelled";
                    await dbContext.SaveChangesAsync(cancellationToken);
                }
                
                await messageActions.CompleteMessageAsync(message, cancellationToken);
                return;
            }
            
            var retryMessage = new ServiceBusMessage(message.Body)
            {
                SessionId = message.SessionId,
                ApplicationProperties =
                {
                    { "EntityType", "orders" },
                    { "ActionType", "retry" },
                    { "CountOfRetry", countOfRetry + 1 }
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
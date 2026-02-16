using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using AzureServiceBus.Functions.Database;
using AzureServiceBus.Functions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;

namespace AzureServiceBus.Functions;

public class OrdersTopicHandler(AppDbContext dbContext, JsonSerializerOptions jsonSerializerOptions, ILogger<OrdersTopicHandler> logger)
{
    [Function("OrdersTopicHandler")]
    public async Task Run(
        [ServiceBusTrigger("ashul-service-bus-topic", "ashul-orders-sub", Connection = "ServiceBusConnectionString", IsSessionsEnabled = true)] ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        logger.LogInformation("Orders Topic Handler Started...");
        
        logger.LogInformation($"Received message: {message.Body}");

        try
        {
            var order = JsonSerializer.Deserialize<OrderMessageModel>(message.Body, jsonSerializerOptions);
            var inventory = await dbContext.InventoryItems.FirstOrDefaultAsync(x => x.ProductName == order.Product);

            if (inventory == null)
            {
                await messageActions.DeadLetterMessageAsync(message, deadLetterReason: "InventoryItemNotFound", deadLetterErrorDescription: "InventoryItemNotFound");
                return;
            }
            
            await messageActions.CompleteMessageAsync(message);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Orders Topic Handler Error");
        }
    }
}
using System;
using Azure.Messaging.ServiceBus;
using AzureServiceBus.Console;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>();

var configuration = builder.Build();

await using var azureServiceBusClient = AzureServiceBusClient.CreateClient(configuration.GetSection("ServiceBusConnectionString").Value);

await using var sender = azureServiceBusClient.CreateSender(configuration.GetSection("ServiceBusQueueName").Value);

while (true)
{
    var message = Console.ReadLine();
    if (message == "`") break;

    using var messageBatch = await sender.CreateMessageBatchAsync();
    messageBatch.TryAddMessage(new ServiceBusMessage(message));
    await sender.SendMessagesAsync(messageBatch);
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AzureServiceBus.Consumer;

public class ProcessQueueMessageService(ILogger<ProcessQueueMessageService> logger, ServiceBusClient serviceBusClient, IConfiguration configuration) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Start listening for queue Messages...");

        await using var processor = serviceBusClient.CreateProcessor(configuration.GetSection("ServiceBusQueueName").Value, new ServiceBusProcessorOptions());

        processor.ProcessMessageAsync += MessageHandler;
        processor.ProcessErrorAsync += async eventArgs =>
        {
            logger.LogError(eventArgs.Exception.Message);

            var receiver = serviceBusClient.CreateReceiver(configuration.GetSection("ServiceBusQueueName").Value, new ServiceBusReceiverOptions()
            {
                ReceiveMode = ServiceBusReceiveMode.PeekLock
            });

            var dlqMessage = eventArgs.Exception switch
            {
                ServiceBusException e => e.ServiceBusReceivedMessage,
                _ => null
            };

            if (dlqMessage == null) return;

            await receiver.DeadLetterMessageAsync(dlqMessage, new Dictionary<string, object>
            {
                { "DeadLetterReason", eventArgs.Exception.Message },
                { "DeadLetterErrorDescription", eventArgs.Exception.StackTrace?.ToString() }
            }, stoppingToken);

            await receiver.DisposeAsync();
        };

        await processor.StartProcessingAsync(stoppingToken);

        var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
        while (await timer.WaitForNextTickAsync(stoppingToken)) { }

        logger.LogInformation("Stop listening for queue Messages.");
        await processor.StopProcessingAsync(stoppingToken);
    }

    async Task MessageHandler(ProcessMessageEventArgs args)
    {
        var message = args.Message.Body.ToString();
        logger.LogInformation(message);

        //throw new ServiceBusException(args.Message, "Error occured while reading message");

        await args.CompleteMessageAsync(args.Message);
    }

}
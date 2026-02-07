using Azure.Messaging.ServiceBus;
using AzureServiceBus.Consumer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(builder
        => builder.AddJsonFile("appSettings.json", true, true)
            .AddUserSecrets<Program>().Build())
    .UseSerilog((c, l) => l.ReadFrom.Configuration(c.Configuration)) ;

builder.ConfigureServices((context, serviceCollection) =>
{
    serviceCollection.AddSingleton(_ => new ServiceBusClient(context.Configuration.GetSection("ServiceBusConnectionString").Value));
    serviceCollection.AddHostedService<ProcessQueueMessageService>();
});

var host = builder.Build();
host.Run();
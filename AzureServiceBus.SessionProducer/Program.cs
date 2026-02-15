using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using AzureServiceBus.SessionProducer;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>();

var configuration = builder.Build();

var client = new ServiceBusClient(configuration.GetSection("ServiceBusConnectionString").Value);

var tasks = new List<Task>
{
    new MessageProducer(client, "Session A", 1000).Run(),
    new MessageProducer(client, "Session B", 1500).Run()
};

Task.WaitAll(tasks);

Console.ReadKey();
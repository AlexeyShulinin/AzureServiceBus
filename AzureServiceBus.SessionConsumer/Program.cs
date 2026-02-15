using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>();

var configuration = builder.Build();

var client = new ServiceBusClient(configuration.GetSection("ServiceBusConnectionString").Value);

var options = new ServiceBusSessionProcessorOptions
{
    Identifier = "AzureServiceBus.SessionConsumer",
    PrefetchCount = 5,
    ReceiveMode = ServiceBusReceiveMode.PeekLock,
    AutoCompleteMessages = false,
    MaxConcurrentSessions = 1,
    MaxConcurrentCallsPerSession = 1,
    SessionIdleTimeout = TimeSpan.FromMinutes(1)
};

await using var processor = client.CreateSessionProcessor("ashul-service-bus-topic", "ashul-default-sub", options);

processor.ProcessMessageAsync += MessageHandler;
processor.ProcessErrorAsync += ErrorHandler;

async Task MessageHandler(ProcessSessionMessageEventArgs args)
{
    var body = args.Message.Body.ToString();
    var sessionState = (await args.GetSessionStateAsync()).ToString();
    
    if (sessionState == "start")
        Console.WriteLine($"New Session Processing started: {args.Message.SessionId}");
    
    Console.WriteLine($"Recieved session: {args.Message.SessionId} state: {sessionState} seq#: {args.Message.SequenceNumber} message: {body}\n");
    
    await args.CompleteMessageAsync(args.Message);

    var isMessageStateExist = args.Message.ApplicationProperties.TryGetValue("State", out var parsedState);
    if (!isMessageStateExist) return;

    var messageState = parsedState.ToString();
    if (messageState == "completed")
    {
        await args.SetSessionStateAsync(new BinaryData(messageState));
        args.ReleaseSession();
        return;
    }
    
    await args.SetSessionStateAsync(new BinaryData(messageState));
}

Task ErrorHandler(ProcessErrorEventArgs args)
{
    Console.WriteLine(args.Exception);
    return Task.CompletedTask;
}

processor.SessionInitializingAsync += SessionInitializingHandler;
processor.SessionClosingAsync += SessionClosingHandler;

async Task SessionInitializingHandler(ProcessSessionEventArgs args)
{
    var sessionSate = (await args.GetSessionStateAsync())?.ToString();

    // if session re-initialized
    if (!string.IsNullOrWhiteSpace(sessionSate))
    {
        Console.WriteLine($"Initializing existed session state: {args.SessionId} state: {sessionSate}");
        return;
    }
    
    Console.WriteLine($"Initializing new session state: {args.SessionId}");
    await args.SetSessionStateAsync(new BinaryData("start"));
}

async Task SessionClosingHandler(ProcessSessionEventArgs args)
{
    var sessionState = (await args.GetSessionStateAsync()).ToString();
    if (sessionState == "completed")
    {
        await args.SetSessionStateAsync(null);
    }
    
    Console.WriteLine($"Closing existed session state: {args.SessionId}");
}

await processor.StartProcessingAsync();
Console.ReadKey();

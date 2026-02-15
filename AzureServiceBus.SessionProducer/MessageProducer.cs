using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace AzureServiceBus.SessionProducer;

public class MessageProducer(ServiceBusClient client, string sessionId, int sendDelay = 1000)
{
    private const int MaxMessageCount = 5;
    
    public async Task Run()
    {
        var options = new ServiceBusSenderOptions()
        {
            Identifier = $"AzureServiceBus.SessionProducer, {sessionId}"
        };
        
        var sender = client.CreateSender("ashul-service-bus-topic", options);

        for (int i = 0; i < MaxMessageCount; i++)
        {
            await Task.Delay(sendDelay);
            var message = new ServiceBusMessage($"Message {i}")
            {
                SessionId = sessionId,
                ApplicationProperties =
                {
                    new KeyValuePair<string, object>("State", i switch
                    {
                        0 => "new",
                        MaxMessageCount - 1 => "completed",
                        _ => "in progress"
                    })
                }
            };
            
            await sender.SendMessageAsync(message);
        }
    }
}
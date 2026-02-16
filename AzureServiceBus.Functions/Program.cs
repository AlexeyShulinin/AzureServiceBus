using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Messaging.ServiceBus;
using AzureServiceBus.Functions.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var host = new HostBuilder()
    .ConfigureAppConfiguration((context, builder) =>
    {
        builder.AddJsonFile("appsettings.json").AddUserSecrets<Program>();
    })
    .ConfigureFunctionsWorkerDefaults()
    .UseSerilog((c, l) => l.ReadFrom.Configuration(c.Configuration))
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton(_ => new ServiceBusClient(context.Configuration["ServiceBusConnectionString"]));
        services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(databaseName: "dbInventory"));
        
        services.AddSingleton(_ =>
        {
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            serializeOptions.Converters.Add(new JsonStringEnumConverter());
            return serializeOptions;
        });
    })
    .Build();
    
var scopeFactory = host.Services.GetRequiredService<IServiceScopeFactory>();
var scope = scopeFactory.CreateScope();
await using (var context = scope.ServiceProvider.GetService<AppDbContext>())
{
    await context.SeedAsync();
}

host.Run();
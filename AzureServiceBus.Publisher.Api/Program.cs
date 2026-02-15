using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Messaging.ServiceBus;
using AzureServiceBus.Publisher.Api;
using AzureServiceBus.Publisher.Api.Azure;
using AzureServiceBus.Publisher.Api.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .UseSerilog((c, l) => l.ReadFrom.Configuration(c.Configuration))
    .ConfigureAppConfiguration(b => b.AddJsonFile("appSettings.json", true, true).AddUserSecrets<Program>().Build());

builder.Services.AddDatabase();
builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddSingleton(_ =>
{
    var serializeOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    serializeOptions.Converters.Add(new JsonStringEnumConverter());
    return serializeOptions;
});

builder.Services.AddSingleton<AzureServiceBusClient>();
builder.Services.AddSingleton(_ => new ServiceBusClient(builder.Configuration["ServiceBusConnectionString"]));

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.RegisterOrdersEndpoints();
app.Run();

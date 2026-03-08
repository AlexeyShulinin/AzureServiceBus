using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Messaging.ServiceBus;
using Azure.Monitor.OpenTelemetry.Exporter;
using AzureServiceBus.Publisher.Api;
using AzureServiceBus.Publisher.Api.Azure;
using AzureServiceBus.Publisher.Api.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .UseSerilog((c, l) => l.ReadFrom.Configuration(c.Configuration))
    .ConfigureAppConfiguration(b => b.AddJsonFile("appSettings.json", true, true).AddUserSecrets<Program>().Build());

builder.Services.AddDatabase(builder.Configuration);
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
builder.Services.AddSingleton(_ =>
{
    var options = new ServiceBusClientOptions
    {
        RetryOptions = new ServiceBusRetryOptions
        {
            Mode = ServiceBusRetryMode.Exponential,
            TryTimeout = TimeSpan.FromSeconds(60),
            MaxRetries = 5
        }
    };
    
    return new ServiceBusClient(builder.Configuration["ServiceBusConnectionString"], options);
});

builder.Services.AddOpenApi();

AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService("AzureServiceBus.Publisher.Api"))

        .AddAspNetCoreInstrumentation(o => o.RecordException = true)
        .AddHttpClientInstrumentation()

        // Manually add ProsgreSQL
        .AddNpgsql()

        // Azure service bus tracing
        .AddSource("Azure.*")

        // Export App Insight
        .AddAzureMonitorTraceExporter(o =>
            o.ConnectionString = builder.Configuration
                ["ApplicationInsights:ConnectionString"]))

    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddAzureMonitorMetricExporter(o =>
            o.ConnectionString = builder.Configuration
                ["ApplicationInsights:ConnectionString"]));

var allowAllOrigins = "allowAllOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowAllOrigins,
        policy  => { policy.WithOrigins("*"); });
});

var app = builder.Build();
app.MapOpenApi();
/*if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}*/

app.UseCors(allowAllOrigins);
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.RegisterOrdersEndpoints();
app.RegisterHealthEndpoints();
app.Run();

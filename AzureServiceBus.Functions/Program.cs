using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Messaging.ServiceBus;
using Azure.Monitor.OpenTelemetry.Exporter;
using AzureServiceBus.Functions.Database;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
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
        AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);
        
        services.AddSingleton(_ => new ServiceBusClient(context.Configuration["ServiceBusConnectionString"]));
        var dataSource = new NpgsqlDataSourceBuilder(context.Configuration.GetConnectionString("PostgreSql"))
                .ConfigureTracing(o => o
                    .ConfigureCommandSpanNameProvider(cmd => cmd.CommandText))
                .Build();

            services.AddSingleton(dataSource);
            services.AddDbContext<AppDbContext>(o =>
            {
                o.UseNpgsql(dataSource).UseLowerCaseNamingConvention();
                
    #if (DEBUG)
                options.EnableSensitiveDataLogging();
    #endif
            });

            services.AddOpenTelemetry()
                // For functions additional call
                .UseFunctionsWorkerDefaults()
                .WithTracing(tracing => tracing
                    .SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService("AzureServiceBus.Functions"))
                    .AddNpgsql()
                    .AddSource("Azure.*")
                    .AddAzureMonitorTraceExporter(o => 
                        o.ConnectionString = context.Configuration["ApplicationInsights:ConnectionString"]))
                .WithMetrics(metrics => metrics
                    .AddHttpClientInstrumentation()
                    .AddAzureMonitorMetricExporter(o =>
                        o.ConnectionString = context.Configuration["ApplicationInsights:ConnectionString"]));
        
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

host.Run();
using System;
using AzureServiceBus.Publisher.Api.Database;
using AzureServiceBus.Publisher.Api.Repositories;
using AzureServiceBus.Publisher.Api.Repositories.Interfaces;
using AzureServiceBus.Publisher.Api.Services;
using AzureServiceBus.Publisher.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace AzureServiceBus.Publisher.Api;

public static class ServiceCollectionExtensions
{
    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var dataSource = new NpgsqlDataSourceBuilder(configuration.GetConnectionString("PostgreSql"))
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
    }
    
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IOrdersRepository, OrdersRepository>();
    }
    
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IOrdersService, OrdersService>();
    }
}
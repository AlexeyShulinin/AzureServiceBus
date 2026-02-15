using AzureServiceBus.Publisher.Api.Database;
using AzureServiceBus.Publisher.Api.Repositories;
using AzureServiceBus.Publisher.Api.Repositories.Interfaces;
using AzureServiceBus.Publisher.Api.Services;
using AzureServiceBus.Publisher.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AzureServiceBus.Publisher.Api;

public static class ServiceCollectionExtensions
{
    public static void AddDatabase(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(databaseName: "dbOrders"));
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
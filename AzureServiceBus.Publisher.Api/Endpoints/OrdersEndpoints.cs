using System.Threading;
using System.Threading.Tasks;
using AzureServiceBus.Publisher.Api.Models.Requests;
using AzureServiceBus.Publisher.Api.Services.Interfaces;
using Microsoft.AspNetCore.Builder;

namespace AzureServiceBus.Publisher.Api.Endpoints;

public static class OrdersEndpoints
{
    public static void RegisterOrdersEndpoints(this WebApplication app)
    {
        var api = app.MapGroup("/api/");

        api.MapPost("orders", CreateOrderAsync)
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                operation.Summary     = "Create new order.";
                operation.Description = "Returns status of operation.";
                return Task.FromResult(operation);
            });
        
        api.MapPut("orders/{orderId}", UpdateOrderAsync)
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                operation.Summary     = "Update new order.";
                operation.Description = "Returns status of operation.";
                return Task.FromResult(operation);
            });
    }
    
    private static Task<string> CreateOrderAsync(CreateOrderRequest orderRequest, IOrdersService ordersService, CancellationToken cancellationToken)
        => ordersService.CreateOrderAsync(orderRequest, cancellationToken);
    
    private static Task<string> UpdateOrderAsync(string orderId, UpdateOrderRequest orderRequest, IOrdersService ordersService, CancellationToken cancellationToken)
        => ordersService.UpdateOrderAsync(orderId, orderRequest, cancellationToken);
}
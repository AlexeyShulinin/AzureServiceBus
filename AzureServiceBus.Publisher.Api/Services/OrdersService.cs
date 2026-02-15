using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AzureServiceBus.Publisher.Api.Azure;
using AzureServiceBus.Publisher.Api.Enums;
using AzureServiceBus.Publisher.Api.Models.Requests;
using AzureServiceBus.Publisher.Api.Repositories.Interfaces;
using AzureServiceBus.Publisher.Api.Repositories.Models;
using AzureServiceBus.Publisher.Api.Resources;
using AzureServiceBus.Publisher.Api.Services.Interfaces;
using AzureServiceBus.Publisher.Api.Services.Mappings;
using Microsoft.Extensions.Logging;

namespace AzureServiceBus.Publisher.Api.Services;

public class OrdersService(IOrdersRepository ordersRepository, AzureServiceBusClient serviceBusClient, ILogger<OrdersService> logger) : IOrdersService
{
    public async Task<string> CreateOrderAsync(CreateOrderRequest orderRequest, CancellationToken cancellationToken)
    {
        var orderDtoModel = orderRequest.MapTo<OrderDtoModel>();
        orderDtoModel.Status = Status.New;
        
        var createdOrder = await ordersRepository.CreateOrderAsync(orderDtoModel, cancellationToken);
        await serviceBusClient.SendOrderMessageAsync(createdOrder, ActionType.Create, cancellationToken);
        
        logger.LogInformation($"Order created: {JsonSerializer.Serialize(createdOrder)}");
        return string.Format(ApiMessages.OrderSuccessfullyCreated,  createdOrder.OrderId);
    }

    public async Task<string> UpdateOrderAsync(string orderId, UpdateOrderRequest orderRequest, CancellationToken cancellationToken)
    {
        var orderDtoModel = orderRequest.MapTo<OrderDtoModel>();

        var updatedOrder = await ordersRepository.UpdateOrderAsync(orderId, orderDtoModel, cancellationToken);
        await serviceBusClient.SendOrderMessageAsync(updatedOrder, ActionType.Update, cancellationToken);
        
        logger.LogInformation($"Order updated: {JsonSerializer.Serialize(updatedOrder)}");
        return string.Format(ApiMessages.OrderSuccessfullyUpdated,  updatedOrder.OrderId);
    }
}
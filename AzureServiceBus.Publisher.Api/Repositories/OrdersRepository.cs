using System;
using System.Threading;
using System.Threading.Tasks;
using AzureServiceBus.Publisher.Api.Database;
using AzureServiceBus.Publisher.Api.Database.Models;
using AzureServiceBus.Publisher.Api.Exceptions;
using AzureServiceBus.Publisher.Api.Repositories.Interfaces;
using AzureServiceBus.Publisher.Api.Repositories.Mappings;
using AzureServiceBus.Publisher.Api.Repositories.Models;
using AzureServiceBus.Publisher.Api.Resources;
using Microsoft.EntityFrameworkCore;

namespace AzureServiceBus.Publisher.Api.Repositories;

public class OrdersRepository(AppDbContext dbContext) : IOrdersRepository
{
    public async Task<OrderDtoModel> CreateOrderAsync(OrderDtoModel order, CancellationToken cancellationToken)
    {
        var dbOrder = order.MapTo<Order>();
        dbOrder.OrderId = $"orders-{Guid.NewGuid().ToString().ToLowerInvariant()}";
        dbOrder.Date = DateOnly.FromDateTime(DateTime.Today);

        dbContext.Add(dbOrder);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return dbOrder.MapTo<OrderDtoModel>();
    }

    public async Task<OrderDtoModel> UpdateOrderAsync(string orderId, OrderDtoModel order, CancellationToken cancellationToken)
    {
        var dbOrder = await dbContext.Orders.FirstOrDefaultAsync(x => x.OrderId == orderId, cancellationToken);
        
        if (dbOrder == null)
            throw new NotFoundException(DatabaseMessages.EntityNotFoundException);
        
        dbOrder.Status = order.Status?.ToString();
        dbOrder.Amount = order.Amount;
        dbOrder.Count = order.Count;
        dbOrder.Date = DateOnly.FromDateTime(DateTime.Today);
        
        await dbContext.SaveChangesAsync(cancellationToken);
        return dbOrder.MapTo<OrderDtoModel>();
    }
}
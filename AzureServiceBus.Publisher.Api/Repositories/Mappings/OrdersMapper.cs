using System;
using AzureServiceBus.Publisher.Api.Database.Models;
using AzureServiceBus.Publisher.Api.Enums;
using AzureServiceBus.Publisher.Api.Repositories.Models;

namespace AzureServiceBus.Publisher.Api.Repositories.Mappings;

public static class OrdersMapper
{
    public static T MapTo<T>(this OrderDtoModel source) where T : Order, new()
        => new()
        {
            OrderId = source.OrderId,
            Product = source.Product,
            Amount = source.Amount,
            Count = source.Count,
            Customer = source.Customer,
            Date = source.Date,
            Status = source.Status?.ToString()
        };
    
    public static T MapTo<T>(this Order source) where T : OrderDtoModel, new()
        => new()
        {
            OrderId = source.OrderId,
            Product = source.Product,
            Amount = source.Amount,
            Count = source.Count,
            Customer = source.Customer,
            Date = source.Date,
            Status = Enum.TryParse<Status>(source.Status, out var result) ? result : null
        };
}
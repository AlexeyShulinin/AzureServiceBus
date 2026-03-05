using System.Collections.Generic;
using System.Linq;
using AzureServiceBus.Publisher.Api.Models.Requests;
using AzureServiceBus.Publisher.Api.Models.Responses;
using AzureServiceBus.Publisher.Api.Repositories.Models;

namespace AzureServiceBus.Publisher.Api.Services.Mappings;

public static class OrdersMapper
{
    public static T MapTo<T>(this CreateOrderRequest source) where T : OrderDtoModel, new()
        => new T
        {
            Product = source.Product,
            Amount = source.Amount,
            Count = source.Count,
            Customer = source.Customer
        };
    
    public static T MapTo<T>(this UpdateOrderRequest source) where T : OrderDtoModel, new()
        => new T
        {
            Amount = source.Amount,
            Count = source.Count,
            Status = source.Status
        };

    private static GetOrderListResponse MapTo(this OrderDtoModel source)
        => new(source.OrderId, source.Product, source.Amount, source.Count, source.Date, source.Status.ToString());
    
    public static List<GetOrderListResponse> MapToList(this List<OrderDtoModel> source)
        => source.Select(MapTo).ToList();
}
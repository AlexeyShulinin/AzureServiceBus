using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureServiceBus.Publisher.Api.Models.Requests;
using AzureServiceBus.Publisher.Api.Models.Responses;

namespace AzureServiceBus.Publisher.Api.Services.Interfaces;

public interface IOrdersService
{
    Task<string> CreateOrderAsync(CreateOrderRequest orderRequest, CancellationToken cancellationToken);

    Task<string> UpdateOrderAsync(string orderId, UpdateOrderRequest orderRequest, CancellationToken cancellationToken);
    
    Task<List<GetOrderListResponse>> GetOrderListAsync(CancellationToken cancellationToken);
}
using System.Threading;
using System.Threading.Tasks;
using AzureServiceBus.Publisher.Api.Repositories.Models;

namespace AzureServiceBus.Publisher.Api.Repositories.Interfaces;

public interface IOrdersRepository
{
    Task<OrderDtoModel> CreateOrderAsync(OrderDtoModel order, CancellationToken cancellationToken);

    Task<OrderDtoModel> UpdateOrderAsync(string orderId, OrderDtoModel order, CancellationToken cancellationToken);
}
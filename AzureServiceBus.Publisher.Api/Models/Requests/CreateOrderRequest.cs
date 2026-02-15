namespace AzureServiceBus.Publisher.Api.Models.Requests;

public record CreateOrderRequest(string Product, decimal Amount, int? Count, string Customer); 
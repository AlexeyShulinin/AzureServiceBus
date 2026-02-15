using AzureServiceBus.Publisher.Api.Enums;

namespace AzureServiceBus.Publisher.Api.Models.Requests;

public record UpdateOrderRequest(decimal Amount, int? Count, Status Status);

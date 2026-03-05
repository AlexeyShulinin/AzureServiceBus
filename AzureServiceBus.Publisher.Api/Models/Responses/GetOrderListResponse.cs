using System;

namespace AzureServiceBus.Publisher.Api.Models.Responses;

public record GetOrderListResponse(string OrderId, string Product, decimal Amount, int? Count, DateOnly Date, string Status);
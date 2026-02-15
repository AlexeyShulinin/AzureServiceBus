using System;
using AzureServiceBus.Publisher.Api.Enums;

namespace AzureServiceBus.Publisher.Api.Repositories.Models;

public class OrderDtoModel
{
    public string OrderId { get; set; }
    
    public string Product { get; set; }
    
    public decimal Amount { get; set; }

    public int? Count { get; set; }

    public string Customer { get; set; }

    public DateOnly Date { get; set; }

    public Status? Status { get; set; }
}
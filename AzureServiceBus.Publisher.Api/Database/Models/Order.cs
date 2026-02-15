using System;

namespace AzureServiceBus.Publisher.Api.Database.Models;

public class Order
{
    public string OrderId { get; set; }
    
    public string Product { get; set; }
    
    public decimal Amount { get; set; }

    public int? Count { get; set; }

    public string Customer { get; set; }

    public DateOnly Date { get; set; }

    public string Status { get; set; }
}
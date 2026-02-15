using System;

namespace AzureServiceBus.Functions.Models;

public class OrderMessageModel
{
    public string OrderId { get; set; }
    
    public string Product { get; set; }
    
    public decimal Amount { get; set; }

    public int? Count { get; set; }

    public string Customer { get; set; }

    public DateOnly Date { get; set; }

    public string Status { get; set; }
}
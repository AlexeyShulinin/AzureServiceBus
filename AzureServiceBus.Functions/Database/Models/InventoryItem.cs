namespace AzureServiceBus.Functions.Database.Models;

public class InventoryItem
{
    public string InventoryId { get; set; }

    public string ProductName { get; set; }
    
    public int? Count { get; set; }
}
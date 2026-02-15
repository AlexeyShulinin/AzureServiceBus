using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureServiceBus.Functions.Database.Models;

namespace AzureServiceBus.Functions.Database;

public static class DatabaseSeedHelper
{
    public static async Task SeedAsync(this AppDbContext context)
    {
        context.SeedInventoryItems();
        await context.SaveChangesAsync();
    }

    private static void SeedInventoryItems(this AppDbContext context)
    {
        context.AddRange(new List<InventoryItem>
        {
            new() { InventoryId = $"inventoryItem-{Guid.NewGuid()}", ProductName = "Lenovo PC", Count = 10 },
            new() { InventoryId = $"inventoryItem-{Guid.NewGuid()}", ProductName = "Mouse Logitech", Count = 1 }
        });
    }
}
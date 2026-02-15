using System.Reflection;
using AzureServiceBus.Functions.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace AzureServiceBus.Functions.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<InventoryItem> InventoryItems { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
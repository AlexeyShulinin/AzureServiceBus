using System.Reflection;
using AzureServiceBus.Publisher.Api.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace AzureServiceBus.Publisher.Api.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
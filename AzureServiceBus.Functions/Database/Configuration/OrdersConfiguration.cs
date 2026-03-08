using AzureServiceBus.Functions.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AzureServiceBus.Functions.Database.Configuration;

public class OrdersConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> entity)
    {
        entity.ToTable("orders");
        entity.HasKey(x => x.OrderId);
    }
}
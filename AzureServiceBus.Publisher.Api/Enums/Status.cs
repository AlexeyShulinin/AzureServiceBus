namespace AzureServiceBus.Publisher.Api.Enums;

public enum Status
{
    New,
    InProcess,
    Processed,
    Delivered,
    Cancelled
}
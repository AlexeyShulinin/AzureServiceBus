namespace AzureServiceBus.Publisher.Api.Enums;

public enum Status
{
    New,
    InProcess,
    Delivered,
    Processed,
    Cancelled
}
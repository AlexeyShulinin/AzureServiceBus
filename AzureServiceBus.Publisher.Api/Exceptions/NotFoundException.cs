namespace AzureServiceBus.Publisher.Api.Exceptions;

public class NotFoundException(string message) : BaseException(message);
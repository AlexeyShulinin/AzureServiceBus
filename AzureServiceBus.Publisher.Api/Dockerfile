FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /AzureServiceBus.Publisher.Api
COPY ["bin/Release/net10.0/publish", "/AzureServiceBus.Publisher.Api"]
ENTRYPOINT ["dotnet", "AzureServiceBus.Publisher.Api.dll"]
# Create a layer docker image starting with base from the aspnet:8.0 image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 11111
EXPOSE 30000

# Create a build layer on the sdk:8.0 image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/product/MonoStore.Product.Host/MonoStore.Product.Host.csproj", "product/MonoStore.Product.Host/"]
COPY ["src/product/MonoStore.Product.Module/MonoStore.Product.Module.csproj", "product/MonoStore.Product.Module/"]
COPY ["src/aspire/MonoStore.ServiceDefaults/MonoStore.ServiceDefaults.csproj", "aspire/MonoStore.ServiceDefaults/"]
COPY ["src/cart/MonoStore.Cart.Contracts/MonoStore.Cart.Contracts.csproj", "cart/MonoStore.Cart.Contracts/"]
RUN dotnet restore "product/MonoStore.Product.Host/MonoStore.Product.Host.csproj"
COPY ./src .
WORKDIR "/src/product/MonoStore.Product.Host"
RUN dotnet build "MonoStore.Product.Host.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "MonoStore.Product.Host.csproj" -c Release -o /app/publish

# Create a final layer with the base image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MonoStore.Product.Host.dll"]
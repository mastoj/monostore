# Create a layer docker image starting with base from the aspnet:9.0 image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 11111
EXPOSE 30000

# Create a build layer on the sdk:9.0 image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/cart/MonoStore.Cart.Host/MonoStore.Cart.Host.csproj", "cart/MonoStore.Cart.Host/"]
COPY ["src/cart/MonoStore.Cart.Module/MonoStore.Cart.Module.csproj", "cart/MonoStore.Cart.Module/"]
COPY ["src/aspire/MonoStore.ServiceDefaults/MonoStore.ServiceDefaults.csproj", "aspire/MonoStore.ServiceDefaults/"]
COPY ["src/product/MonoStore.Product.Contracts/MonoStore.Product.Contracts.csproj", "product/MonoStore.Product.Contracts/"]
RUN dotnet restore "cart/MonoStore.Cart.Host/MonoStore.Cart.Host.csproj"
COPY ./src .
WORKDIR "/src/cart/MonoStore.Cart.Host"
RUN dotnet build "MonoStore.Cart.Host.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "MonoStore.Cart.Host.csproj" -c Release -o /app/publish

# Create a final layer with the base image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MonoStore.Cart.Host.dll"]
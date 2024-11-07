# Create a layer docker image starting with base from the aspnet:8.0 image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 11111
EXPOSE 30000

# Create a build layer on the sdk:8.0 image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/MonoStore.Api/MonoStore.Api.csproj", "MonoStore.Api/"]
COPY ["src/aspire/MonoStore.ServiceDefaults/MonoStore.ServiceDefaults.csproj", "aspire/MonoStore.ServiceDefaults/"]
COPY ["src/cart/MonoStore.Cart.Api/MonoStore.Cart.Api.csproj", "cart/MonoStore.Cart.Api/"]
COPY ["src/product/MonoStore.Product.Api/MonoStore.Product.Api.csproj", "product/MonoStore.Product.Api/"]
COPY ["src/cart/MonoStore.Cart.Contracts/MonoStore.Cart.Contracts.csproj", "cart/MonoStore.Cart.Contracts/"]
COPY ["src/product/MonoStore.Product.Contracts/MonoStore.Product.Contracts.csproj", "product/MonoStore.Product.Contracts/"]
RUN dotnet restore "MonoStore.Api/MonoStore.Api.csproj"
COPY ./src .
WORKDIR "/src/MonoStore.Api"
RUN dotnet build "MonoStore.Api.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "MonoStore.Api.csproj" -c Release -o /app/publish

# Create a final layer with the base image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MonoStore.Api.dll"]
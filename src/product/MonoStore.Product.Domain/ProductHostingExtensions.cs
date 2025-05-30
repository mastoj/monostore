using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MonoStore.Product.Domain;

public static class ProductHostingExtensions
{
  public static IHostApplicationBuilder AddProductService(this IHostApplicationBuilder builder)
  {
    var connectionString = Environment.GetEnvironmentVariable("COSMOS_CONNECTION_STRING");
    builder.Services.AddSingleton(new CosmosClient(connectionString));
    builder.Services.AddSingleton<ProductRepository>();
    return builder;
  }
}

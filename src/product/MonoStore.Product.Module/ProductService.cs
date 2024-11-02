using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using MonoStore.Product.Contracts;

namespace MonoStore.Product.Module;

public record ProductCosmosDto(
  string id,
  string value
);

public class ProductService : IProductService
{
  private CosmosClient cosmosClient;
  private readonly ILogger<ProductService> logger;

  public ProductService(CosmosClient cosmosClient, ILogger<ProductService> logger)
  {
    this.cosmosClient = cosmosClient;
    this.logger = logger;
  }
  // Func<string, string> getOperatingChain = (id) =>
  // {
  //   return id.Split("_")[0];
  // };

  // var operatingChain = getOperatingChain(id);
  // var container = cosmosClient.GetContainer("ecom-data", $"product-ecom-feed4-dotnet-{operatingChain}");
  // var result = await container.ReadItemStreamAsync(id, new PartitionKey(id));
  // // Use id as key to query product
  // if (result.StatusCode == System.Net.HttpStatusCode.NotFound)
  // {
  //   return Results.NotFound();
  // }

  private static string databaseName = "ecom-data";
  private static string containerId = "product-ecom-feed4-dotnet";
  public async Task<ProductDetail> GetProductAsync(string sku, string operatingChain)
  {
    throw new NotImplementedException();
    // var container = cosmosClient.GetContainer(databaseName, $"{containerId}-{operatingChain}");
    // var key = $"{operatingChain}_b2c_{sku}";
    // var result = await container.ReadItemAsync<ProductCosmosDto>(key, new PartitionKey(key));
    // logger.LogInformation($"GetProductAsync: {key}");
    // logger.LogInformation($"GetProductAsync: {result.Resource.value}");
    // return new ProductDetail
    // {
    //   Sku = sku,
    //   OperatingChain = operatingChain,
    //   Name = result.Resource.value,
    //   Price = 123,
    //   PriceExclVat = 133
    // };
  }
}

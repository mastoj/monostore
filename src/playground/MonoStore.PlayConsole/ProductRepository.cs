using Microsoft.Azure.Cosmos;
using MonoStore.Product.Contracts;

namespace MonoStore.PlayConsole;

public class ProductRepository
{
  private readonly CosmosClient _cosmosClient;
  public ProductRepository(CosmosClient client)
  {
    _cosmosClient = client;
  }

  private Database _database;
  private async Task<Database> GetDatabase()
  {
    if (_database == null)
    {
      _database = await _cosmosClient.CreateDatabaseIfNotExistsAsync("ecom-data");
    }
    return _database;
  }

  private Container _container;
  private async Task<Container> GetContainer()
  {
    if (_container == null)
    {
      var database = await GetDatabase();
      _container = await database.CreateContainerIfNotExistsAsync("monostore-products", "/OperatingChain");
    }
    return _container;
  }

  public async Task<ProductDetail> GetProductAsync(string sku, string operatingChain)
  {
    var container = await GetContainer();
    var product = await container.ReadItemAsync<ProductDetail>($"{operatingChain}_{sku}", new PartitionKey(operatingChain));
    return product.Resource;
  }

  public async Task WriteProducts(IEnumerable<ProductDetail> products)
  {
    var container = await GetContainer();
    var groups = products.GroupBy(i => i.OperatingChain);
    var batchOperations = groups.SelectMany(group =>
    {
      var chunks = group.Chunk(100);
      var chunkOperations = chunks.Select(async chunk =>
      {
        var batch = container.CreateTransactionalBatch(new PartitionKey(group.Key));
        foreach (var product in chunk)
        {
          batch.UpsertItem(product);
        }
        var result = await batch.ExecuteAsync();
        if (!result.IsSuccessStatusCode)
        {
          Console.WriteLine("==> Failed to execute batch operations: " + result.StatusCode);
        }
      });
      return chunkOperations;
    });
    await Task.WhenAll(batchOperations);
  }
}
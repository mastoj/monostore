namespace MonoStore.Product.Domain;

using Microsoft.Azure.Cosmos;
using MonoStore.Contracts.Product;
using System.Text.Json;

public class ProductRepository(CosmosClient client)
{
  private readonly CosmosClient _cosmosClient = client;
  private Database? _database;
  private async Task<Database> GetDatabase()
  {
    if (_database == null)
    {
      _database = await _cosmosClient.CreateDatabaseIfNotExistsAsync("ecom-data");
    }
    return _database;
  }

  private Container? _container;
  private async Task<Container> GetContainer()
  {
    if (_container == null)
    {
      var database = await GetDatabase();
      _container = await database.CreateContainerIfNotExistsAsync("monostore-products", "/OperatingChain");
    }
    return _container;
  }

  public async Task<ProductDetail> GetProductAsync(string operatingChain, string sku)
  {
    try
    {

      var container = await GetContainer();
      var product = await container.ReadItemAsync<ProductDetail>($"{operatingChain}_{sku}", new PartitionKey(operatingChain));
      return product.Resource;
    }
    catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
    {
      Console.WriteLine("==> Failed to get product (not found): ");
      return null;
    }
    catch (Exception ex)
    {
      Console.WriteLine("==> Failed to get product: " + ex.Message);
      return null;
    }
  }

  public async Task<int> DumpProductsToFile()
  {
    try
    {
      var container = await GetContainer();
      var products = new List<ProductDetail>();

      // Query all products from the container
      var queryDefinition = new QueryDefinition("SELECT * FROM c");
      using var feedIterator = container.GetItemQueryIterator<ProductDetail>(queryDefinition);

      while (feedIterator.HasMoreResults)
      {
        var response = await feedIterator.ReadNextAsync();
        products.AddRange(response);
      }

      // Serialize to JSON
      var jsonOptions = new JsonSerializerOptions
      {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
      };

      var json = JsonSerializer.Serialize(products, jsonOptions);

      // Write to file
      var fileName = $"products_dump_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";
      var filePath = Path.Combine(Environment.CurrentDirectory, ".dump", fileName);
      var directory = Path.GetDirectoryName(filePath);
      if (!Directory.Exists(directory) && directory != null)
      {
        Directory.CreateDirectory(directory);
      }
      await File.WriteAllTextAsync(filePath, json);

      Console.WriteLine($"==> Products dumped to file: {filePath}");
      Console.WriteLine($"==> Total products exported: {products.Count}");
      return products.Count;
    }
    catch (Exception ex)
    {
      Console.WriteLine($"==> Failed to dump products to file: {ex.Message}");
      throw;
    }
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
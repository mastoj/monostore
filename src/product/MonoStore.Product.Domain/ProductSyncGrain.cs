using Microsoft.Extensions.Logging;
using MonoStore.Product.Domain;
using Orleans.Streams;

namespace MonoStore.Contracts.Product.Grains
{
  public class ProductSyncGrain : Grain, IProductSyncGrain
  {
    private IAsyncStream<ProductSyncEvent> stream;

    private readonly ProductRepository repository;

    public ProductSyncGrain(ILogger<ProductSyncGrain> logger, ProductRepository repository)
    {
      this.repository = repository;
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
      var streamProvider = this.GetStreamProvider("ProductStreamProvider");
      stream = streamProvider.GetStream<ProductSyncEvent>("ProductSyncStream");
      return base.OnActivateAsync(cancellationToken);
    }

    public async Task SyncProductAsync(List<ProductDetail> products)
    {
      Console.WriteLine("==> SyncProductAsync: " + products.Count);
      await repository.WriteProducts(products);
      var productGrainIds = products.Select(p => IProductGrain.ProductGrainId(p.OperatingChain, p.ArticleNumber)).ToList();
      await stream.OnNextAsync(new ProductSyncEvent
      {
        ProductGrainIds = productGrainIds
      });
    }

    public async Task<int> DumpProductsToFileAsync()
    {
      Console.WriteLine("==> Dumping products to file");
      var productCount = await repository.DumpProductsToFile();
      return productCount;
    }
  }
}
using Microsoft.Extensions.Logging;
using Monostore.ServiceDefaults;
using MonoStore.Contracts.Product;
using MonoStore.Contracts.Product.Grains;
using Orleans.Streams;

namespace MonoStore.Product.Module;

public class ProductGrain : Grain, IProductGrain
{
  private ProductDetail? state;
  private readonly ProductRepository repository;

  public ProductGrain(IGrainFactory grainFactory, ProductRepository repository)
  {
    this.repository = repository;
  }
  public override async Task OnActivateAsync(CancellationToken cancellationToken)
  {
    var primaryKeyString = this.GetPrimaryKeyString();
    try
    {

      Console.WriteLine("==> Product: OnActivateAsync");
      state = await GetProduct();
      var streamProvider = this.GetStreamProvider("ProductStreamProvider");
      var stream = streamProvider.GetStream<ProductSyncEvent>("ProductSyncStream");
      await stream.SubscribeAsync(OnNextAsync);

      DiagnosticConfig.ProductHost.ActiveProductCounter.Add(1, new KeyValuePair<string, object?>("operatingChain", state.OperatingChain ?? ""));
    }
    catch (Exception ex)
    {
      Console.WriteLine("==> Product: OnActivateAsync: " + primaryKeyString);
      throw;
    }
    await base.OnActivateAsync(cancellationToken);
  }

  public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
  {
    DiagnosticConfig.ProductHost.ActiveProductCounter.Add(1, new KeyValuePair<string, object?>("operatingChain", state?.OperatingChain));
    await base.OnDeactivateAsync(reason, cancellationToken);
  }
  public Task<ProductDetail> GetProductAsync()
  {
    Console.WriteLine("==> GetProductAsync");
    return Task.FromResult(state!);
  }

  private async Task OnNextAsync(ProductSyncEvent productSyncEvent, StreamSequenceToken token)
  {
    await UpdateProduct(productSyncEvent.ProductGrainIds);
  }

  public async Task UpdateProduct(IEnumerable<string> productIds)
  {
    if (productIds.Contains(this.GetPrimaryKeyString()))
    {
      state = await GetProduct();
    }
  }

  private async Task<ProductDetail> GetProduct()
  {
    var primaryKeyString = this.GetPrimaryKeyString();
    var id = primaryKeyString.Split("/")[1];
    var parts = id.Split("_");
    return await repository.GetProductAsync(parts[0], parts[1]);
  }
}

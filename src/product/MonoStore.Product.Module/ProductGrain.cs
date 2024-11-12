using Monostore.ServiceDefaults;
using MonoStore.Product.Contracts;
using MonoStore.Product.Contracts.Grains;

namespace MonoStore.Product.Module;

public class ProductGrain : Grain, IProductGrain
{
  private ProductDetail? state;
  private readonly ProductRepository repository;

  public ProductGrain(ProductRepository repository)
  {
    this.repository = repository;
  }
  public override async Task OnActivateAsync(CancellationToken cancellationToken)
  {
    var primaryKeyString = this.GetPrimaryKeyString();
    try
    {

      Console.WriteLine("==> Product: OnActivateAsync");

      var id = primaryKeyString.Split("/")[1];
      var parts = id.Split("_");
      state = await repository.GetProductAsync(parts[0], parts[1]);
      DiagnosticConfig.ProductHost.ActiveProductCounter.Add(1, new KeyValuePair<string, object?>("operatingChain", state.OperatingChain ?? ""));
    }
    catch (Exception)
    {
      Console.WriteLine("==> Product: OnActivateAsync: " + primaryKeyString);
      throw;
    }
  }

  public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
  {
    DiagnosticConfig.ProductHost.ActiveProductCounter.Add(1, new KeyValuePair<string, object?>("operatingChain", state?.OperatingChain));

    return base.OnDeactivateAsync(reason, cancellationToken);
  }
  public Task<ProductDetail> GetProductAsync()
  {
    Console.WriteLine("==> GetProductAsync");
    return Task.FromResult(state!);
  }
}

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
    Console.WriteLine("==> Product: OnActivateAsync");
    var id = this.GetPrimaryKeyString().Split("/")[1];
    var parts = id.Split("_");
    state = await repository.GetProductAsync(parts[0], parts[1]);
  }

  public Task<ProductDetail> GetProductAsync()
  {
    Console.WriteLine("==> GetProductAsync");
    return Task.FromResult(state!);
  }
}

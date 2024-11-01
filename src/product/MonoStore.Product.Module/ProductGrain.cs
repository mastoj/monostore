using MonoStore.Product.Contracts;
using MonoStore.Product.Contracts.Grains;

namespace MonoStore.Product.Module;

public class ProductGrain : Grain, IProductGrain
{
  private ProductDto? state;
  public override async Task OnActivateAsync(CancellationToken cancellationToken)
  {
    //    DelayDeactivation(TimeSpan.FromMinutes(10));
    var id = this.GetPrimaryKeyString().Split("/")[1];
    var parts = this.GetPrimaryKeyString().Split("_");
    var operatingChain = parts[0];
    var sku = parts[1];
    state = new ProductDto
    {
      Sku = sku,
      OperatingChain = operatingChain,
      Name = "Hello",
      Price = 123,
      PriceExclVat = 133
    };
    await base.OnActivateAsync(cancellationToken);
  }

  public Task<ProductDto> GetProductAsync()
  {
    return Task.FromResult(state!);
  }

  public Task<ProductDto> UpdateProductAsync(ProductDto product)
  {
    return Task.FromResult(product);
  }
}

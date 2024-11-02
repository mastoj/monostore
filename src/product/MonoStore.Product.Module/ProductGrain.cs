using MonoStore.Product.Contracts;
using MonoStore.Product.Contracts.Grains;

namespace MonoStore.Product.Module;

public class ProductGrain : Grain, IProductGrain
{
  private ProductDetail? state;
  public override async Task OnActivateAsync(CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
    // //    DelayDeactivation(TimeSpan.FromMinutes(10));
    // var id = this.GetPrimaryKeyString().Split("/")[1];
    // var parts = this.GetPrimaryKeyString().Split("_");
    // var operatingChain = parts[0];
    // var sku = parts[1];
    // state = new ProductDetail
    // {
    //   Sku = sku,
    //   OperatingChain = operatingChain,
    //   Name = "Hello",
    //   Price = 123,
    //   PriceExclVat = 133
    // };
    //    await base.OnActivateAsync(cancellationToken);
  }

  public Task<ProductDetail> GetProductAsync()
  {
    return Task.FromResult(state!);
  }

  public Task<ProductDetail> UpdateProductAsync(ProductDetail product)
  {
    return Task.FromResult(product);
  }
}

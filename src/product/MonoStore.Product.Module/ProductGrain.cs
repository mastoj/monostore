using MonoStore.Product.Contracts;
using MonoStore.Product.Contracts.Grains;

namespace MonoStore.Product.Module;

public class ProductGrain : Grain, IProductGrain
{
  public Task<ProductDto> GetProductAsync(string sku, string operatingChain)
  {
    return Task.FromResult(new ProductDto
    {
      Sku = sku,
      OperatingChain = operatingChain,
      Name = "Hello",
      Price = 123,
      PriceExclVat = 133
    });
  }

  public Task<ProductDto> UpdateProductAsync(ProductDto product)
  {
    return Task.FromResult(product);
  }
}

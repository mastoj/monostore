namespace MonoStore.Product.Contracts.Grains;

public interface IProductGrain : IGrainWithStringKey
{
  Task<ProductDto> GetProductAsync(string sku, string operatingChain);
  Task<ProductDto> UpdateProductAsync(ProductDto product);
}

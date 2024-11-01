namespace MonoStore.Product.Contracts.Grains;

public interface IProductGrain : IGrainWithStringKey
{
  Task<ProductDto> GetProductAsync();
  Task<ProductDto> UpdateProductAsync(ProductDto product);
}

namespace MonoStore.Product.Contracts.Grains;

public interface IProductGrain : IGrainWithStringKey
{
  Task<ProductDetail> GetProductAsync();
  // Task<ProductDetail> UpdateProductAsync(ProductDetail product);
}

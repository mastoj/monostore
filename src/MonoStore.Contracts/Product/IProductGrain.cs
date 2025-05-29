namespace MonoStore.Contracts.Product.Grains;

public interface IProductGrain : IGrainWithStringKey
{
  Task<ProductDetail> GetProductAsync();
  // Task<ProductDetail> UpdateProductAsync(ProductDetail product);
  public static string ProductGrainId(string operatingChain, string id) => $"product/{operatingChain.ToUpper()}_{id}";

}

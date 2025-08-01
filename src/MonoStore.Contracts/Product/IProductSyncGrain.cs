namespace MonoStore.Contracts.Product.Grains;

public interface IProductSyncGrain : IGrainWithStringKey
{
  Task SyncProductAsync(List<ProductDetail> product);
  Task<int> DumpProductsToFileAsync();
  public static string ProductSyncGrainId() => $"productsync/{Guid.Empty}";
}

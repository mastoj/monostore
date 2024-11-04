namespace MonoStore.Product.Contracts.Grains;

public interface IProductSyncGrain : IGrainWithStringKey
{
  Task SyncProductAsync(ProductDto product);
}

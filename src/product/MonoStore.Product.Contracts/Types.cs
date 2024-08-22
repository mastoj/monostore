namespace MonoStore.Product.Contracts;

public record ProductDto(
  string sku,
  string operatingChain,
  string name,
  decimal price,
  decimal priceExclVat
);

public interface IProductService
{
  Task<ProductDto> GetProductAsync(string sku, string operatingChain);
}
namespace MonoStore.Product.Contracts;

[GenerateSerializer, Alias(nameof(ProductDto))]
public record class ProductDto
{
  [Id(0)]
  public string Sku { get; set; } = "";
  [Id(1)]
  public string OperatingChain { get; set; } = "";
  [Id(2)]
  public string Name { get; set; } = "";
  [Id(3)]
  public decimal Price { get; set; } = 0;
  [Id(4)]
  public decimal PriceExclVat { get; set; } = 0;
}


public interface IProductService
{
  Task<ProductDto> GetProductAsync(string sku, string operatingChain);
}
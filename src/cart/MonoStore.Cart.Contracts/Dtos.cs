namespace MonoStore.Cart.Contracts.Dtos;

public enum CartStatus
{
  Open,
  TimedOut,
  Abandoned,
  Archived,
}

[GenerateSerializer, Alias(nameof(Product))]
public record class Product
{
  [Id(0)]
  public string Id { get; set; } = "";
  [Id(1)]
  public string Name { get; set; } = "";
  [Id(2)]
  public decimal Price { get; set; } = 0;
  [Id(3)]
  public decimal PriceExVat { get; set; } = 0;
  [Id(4)]
  public string Url { get; set; } = "";
  [Id(5)]
  public string PrimaryImageUrl { get; set; } = "";
}
[GenerateSerializer, Alias(nameof(CartItem))]
public record class CartItem
{
  [Id(0)]
  public Product Product { get; set; } = new Product();
  [Id(1)]
  public int Quantity { get; set; } = 0;
}

[GenerateSerializer, Alias(nameof(CartData))]
public record class CartData
{
  [Id(0)]
  public Guid Id { get; set; } = Guid.Empty;
  [Id(1)]
  public string OperatingChain { get; set; } = "";
  [Id(2)]
  public CartStatus Status { get; set; } = CartStatus.Open;
  [Id(3)]
  public List<CartItem> Items { get; set; } = new List<CartItem>();
}

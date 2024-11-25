namespace MonoStore.Cart.Contracts;

#region requests if contracts does not match
public record AddItemRequest(Guid CartId, string OperatingChain, string ProductId);
#endregion

public enum CartStatus
{
  Open,
  TimedOut,
  Paid,
}

[GenerateSerializer, Alias(nameof(CreateCart))]
public record class CreateCart
{
  [Id(0)]
  public Guid CartId { get; set; } = Guid.Empty;
  [Id(1)]
  public string OperatingChain { get; set; } = "";
}

[GenerateSerializer, Alias(nameof(GetCart))]
public record class GetCart
{
  [Id(0)]
  public Guid CartId { get; set; } = Guid.Empty;
}

[GenerateSerializer, Alias(nameof(AddItem))]
public record class AddItem
{
  [Id(0)]
  public Guid CartId { get; set; } = Guid.Empty;
  [Id(1)]
  public CartItem Item { get; set; } = new CartItem();
}
[GenerateSerializer, Alias(nameof(RemoveItem))]
public record class RemoveItem
{
  [Id(0)]
  public Guid CartId { get; set; } = Guid.Empty;
  [Id(1)]
  public string ProductId { get; set; } = "";
}
[GenerateSerializer, Alias(nameof(IncreaseItemQuantity))]
public record class IncreaseItemQuantity
{
  [Id(0)]
  public Guid CartId { get; set; } = Guid.Empty;
  [Id(1)]
  public string ProductId { get; set; } = "";
}
[GenerateSerializer, Alias(nameof(DecreaseItemQuantity))]
public record class DecreaseItemQuantity
{
  [Id(0)]
  public Guid CartId { get; set; } = Guid.Empty;
  [Id(1)]
  public string ProductId { get; set; } = "";
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

[GenerateSerializer, Alias(nameof(Cart))]
public record class Cart
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

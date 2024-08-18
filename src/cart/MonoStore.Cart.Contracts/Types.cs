using Orleans;

namespace MonoStore.Cart.Contracts;

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
  public string CartId { get; set; } = "";
}
[GenerateSerializer, Alias(nameof(AddItem))]
public record class AddItem
{
  [Id(0)]
  public string CartId { get; set; } = "";
  [Id(1)]
  public string ProductId { get; set; } = "";
}
[GenerateSerializer, Alias(nameof(RemoveItem))]
public record class RemoveItem
{
  [Id(0)]
  public string CartId { get; set; } = "";
  [Id(1)]
  public string ProductId { get; set; } = "";
}
[GenerateSerializer, Alias(nameof(IncreaseItemQuantity))]
public record class IncreaseItemQuantity
{
  [Id(0)]
  public string CartId { get; set; } = "";
  [Id(1)]
  public string ItemId { get; set; } = "";
}
[GenerateSerializer, Alias(nameof(DecreaseItemQuantity))]
public record class DecreaseItemQuantity
{
  [Id(0)]
  public string CartId { get; set; } = "";
  [Id(1)]
  public string ItemId { get; set; } = "";
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
  public string Id { get; set; } = "";
  [Id(1)]
  public CartStatus Status { get; set; } = CartStatus.Open;
  [Id(2)]
  public IEnumerable<CartItem> Items { get; set; } = new List<CartItem>();
}

using MonoStore.Cart.Module.Facades;

namespace MonoStore.Cart.Module;

#region Types
public record Product(string ProductId, string Name, decimal Price, decimal PriceExVat);
public record CartItem(Product Product, int Quantity);

public record CartCreated(Guid CartId);
public enum CartStatus
{
  Open,
  TimedOut,
  Paid,
}
#endregion

#region Events
public record ItemAddedToCart(Guid CartId, Product Product);
public record ItemRemovedFromCart(Guid CartId, string ProductId);
public record ItemQuantityIncreased(Guid CartId, string ProductId);
public record ItemQuantityDecreased(Guid CartId, string ProductId);
#endregion

public record Cart(
    Guid Id,
    CartStatus Status,
    IEnumerable<CartItem> Items,
    int Version = 1
)

{
  public static Cart Create(CartCreated action) =>
      new(action.CartId, CartStatus.Open, new List<CartItem>());

  public Cart Apply(ItemAddedToCart action) =>
      this with
      {
        Items = Items.Append(new CartItem(action.Product, 1)),
      };

  public Cart Apply(ItemRemovedFromCart action) =>
      this with
      {
        Items = Items.Where(i => i.Product.ProductId != action.ProductId),
      };

  private static IEnumerable<CartItem> UpdateCartItemsQuantity(IEnumerable<CartItem> items, string productId, int change) =>
      items.Select(item => item.Product.ProductId == productId ? item with { Quantity = item.Quantity + change } : item);
  public Cart Apply(ItemQuantityIncreased action) =>
      this with
      {
        Items = UpdateCartItemsQuantity(Items, action.ProductId, 1)
      };

  public Cart Apply(ItemQuantityDecreased action) =>
      this with
      {
        Items = UpdateCartItemsQuantity(Items, action.ProductId, -1)
      };
}
using MonoStore.Cart.Module.Facades;

namespace MonoStore.Cart.Module;

#region Types
public record Product(string ProductId, string Name, decimal Price, decimal PriceExVat);
internal record CartItem(Product Product, int Quantity);

internal record CartCreated(Guid CartId);
internal enum CartStatus
{
  Open,
  TimedOut,
  Paid,
}
#endregion

#region Events
internal record ItemAddedToCart(Guid CartId, Product Product);
internal record ItemRemovedFromCart(Guid CartId, string ProductId);
internal record ItemQuantityIncreased(Guid CartId, string ProductId);
internal record ItemQuantityDecreased(Guid CartId, string ProductId);
#endregion

internal record Cart(
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
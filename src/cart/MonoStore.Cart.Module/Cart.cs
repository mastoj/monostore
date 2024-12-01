using MonoStore.Cart.Contracts;

namespace MonoStore.Cart.Module;

#region Events
public record CartCreated(Guid CartId, string OperatingChain);
public record ItemAddedToCart(Guid CartId, CartItem Item);
public record ItemRemovedFromCart(Guid CartId, string ProductId);
public record ItemQuantityIncreased(Guid CartId, string ProductId);
public record ItemQuantityDecreased(Guid CartId, string ProductId);
#endregion

public record Cart(
    Guid Id,
    string OperatingChain,
    CartStatus Status,
    IEnumerable<CartItem> Items,
    int Version = 1
)

{
  public static Cart Create(CartCreated action) =>
      new(action.CartId, action.OperatingChain, CartStatus.Open, new List<CartItem>());

  public Cart Apply(ItemAddedToCart action) =>
      this with
      {
        Items = Items.Append(action.Item),
      };

  public Cart Apply(ItemRemovedFromCart action) =>
      this with
      {
        Items = Items.Where(i => i.Product.Id != action.ProductId),
      };

  private static IEnumerable<CartItem> UpdateCartItemsQuantity(IEnumerable<CartItem> items, string productId, int change) =>
      items.Select(item => item.Product.Id == productId ? item with { Quantity = item.Quantity + change } : item);
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
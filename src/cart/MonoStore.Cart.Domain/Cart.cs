using MonoStore.Cart.Contracts;
using MonoStore.Cart.Contracts.Dtos;

namespace MonoStore.Cart.Domain;

#region Events
public record CartCreated(Guid CartId, string OperatingChain, string SessionId, string? UserId);
public record ItemAddedToCart(Guid CartId, CartItem Item);
public record ItemRemovedFromCart(Guid CartId, string ProductId);
public record ItemQuantityChanged(Guid CartId, string ProductId, int Quantity);
public record CartAbandoned(Guid CartId);
public record CartRecovered(Guid CartId);
public record CartArchived(Guid CartId);
public record CartCleared(Guid CartId);
#endregion

public record Cart(
    Guid Id,
    string OperatingChain,
    CartStatus Status,
    List<CartItem> Items,
    string SessionId,
    string? UserId,
    decimal Total,
    decimal TotalExVat = 0,
    decimal BeforePriceTotal = 0,
    decimal BeforePriceExVatTotal = 0,
    string Currency = "",
    int Version = 1
)

{
  public static Cart Create(CartCreated action) =>
      new(action.CartId, action.OperatingChain, CartStatus.Open, new List<CartItem>(), action.SessionId, action.UserId, 0, Currency: "NOK");

  private static Cart ApplyPriceChanges(Cart cart) =>
      cart with
      {
        Total = cart.Items.Sum(i => i.Product.Price * i.Quantity),
        TotalExVat = cart.Items.Sum(i => i.Product.PriceExVat * i.Quantity),
        BeforePriceTotal = cart.Items?.Sum(i => i.Product.BeforePrice * i.Quantity) ?? 0,
        BeforePriceExVatTotal = cart.Items?.Sum(i => i.Product.BeforePriceExVat * i.Quantity) ?? 0
      };

  public Cart Apply(ItemAddedToCart action) => ApplyPriceChanges(
      this with
      {
        Items = Items.Append(action.Item).ToList(),
        Version = Version + 1
      }
  );

  public Cart Apply(ItemRemovedFromCart action) => ApplyPriceChanges(
      this with
      {
        Items = Items.Where(i => i.Product.Id != action.ProductId).ToList(),
        Version = Version + 1
      });

  private static IEnumerable<CartItem> UpdateCartItemsQuantity(IEnumerable<CartItem> items, string productId, int newQuantity) =>
      items.Select(item => item.Product.Id == productId ? item with { Quantity = newQuantity } : item);
  public Cart Apply(ItemQuantityChanged action) => ApplyPriceChanges(
      this with
      {
        Items = UpdateCartItemsQuantity(Items, action.ProductId, action.Quantity).ToList(),
        Version = Version + 1
      }
  );

  public Cart Apply(CartAbandoned _) =>
      this with
      {
        Status = CartStatus.Abandoned,
        Version = Version + 1
      };

  public Cart Apply(CartRecovered _) =>
      this with
      {
        Status = CartStatus.Open,
        Version = Version + 1
      };

  public Cart Apply(CartArchived _) =>
      this with
      {
        Status = CartStatus.Archived,
        Version = Version + 1
      };
  public Cart Apply(CartCleared _) => ApplyPriceChanges(this with { Items = new List<CartItem>() });
}
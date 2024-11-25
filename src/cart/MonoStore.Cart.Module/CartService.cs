using DotNext;
using MonoStore.Cart.Contracts;

namespace MonoStore.Cart.Module;

// internal record CreateCart(
//     Guid CartId
// );

// internal record AddItem(
//     Guid CartId,
//     Product Product
// );

// internal record RemoveItem(
//     Guid CartId,
//     string ProductId
// );

// internal record IncreaseItemQuantity(
//     Guid CartId,
//     string ProductId
// );

// internal record DecreaseItemQuantity(
//     Guid CartId,
//     string ProductId
// );

internal static class CartService
{
  private static bool IsItemInCart(Cart cart, string productId) =>
      cart.Items.Any(item => item.Product.Id == productId);

  private static bool IsCartPaid(Cart cart) =>
      cart.Status == CartStatus.Paid;

  internal static Result<CartCreated> Create(CreateCart command)
  {
    // Todo: check that cart does not already exist
    return Result.FromValue(new CartCreated(command.CartId, command.OperatingChain));
  }

  internal static Result<ItemAddedToCart> Handle(Cart current, AddItem command)
  {
    if (IsCartPaid(current))
    {
      return Result.FromException<ItemAddedToCart>(new InvalidOperationException("Cannot add items to a paid cart"));
    }
    if (IsItemInCart(current, command.Item.Product.Id))
    {
      return Result.FromException<ItemAddedToCart>(new InvalidOperationException("Item already in cart"));
    }
    return Result.FromValue(new ItemAddedToCart(current.Id, command.Item));
  }

  internal static Result<ItemRemovedFromCart> Handle(Cart current, RemoveItem command)
  {
    if (IsCartPaid(current))
    {
      return Result.FromException<ItemRemovedFromCart>(new InvalidOperationException("Cannot remove items from a paid cart"));
    }
    if (!IsItemInCart(current, command.ProductId))
    {
      return Result.FromException<ItemRemovedFromCart>(new InvalidOperationException("Item not in cart"));
    }
    return Result.FromValue(new ItemRemovedFromCart(current.Id, command.ProductId));
  }

  internal static Result<ItemQuantityIncreased> Handle(Cart current, IncreaseItemQuantity command)
  {
    if (IsCartPaid(current))
    {
      return Result.FromException<ItemQuantityIncreased>(new InvalidOperationException("Cannot increase quantity of items in a paid cart"));
    }
    if (!IsItemInCart(current, command.ProductId))
    {
      return Result.FromException<ItemQuantityIncreased>(new InvalidOperationException("Item not in cart"));
    }
    return Result.FromValue(new ItemQuantityIncreased(current.Id, command.ProductId));
  }

  internal static Result<ItemQuantityDecreased> Handle(Cart current, DecreaseItemQuantity command)
  {
    if (IsCartPaid(current))
    {
      return Result.FromException<ItemQuantityDecreased>(new InvalidOperationException("Cannot decrease quantity of items in a paid cart"));
    }
    if (!IsItemInCart(current, command.ProductId))
    {
      return Result.FromException<ItemQuantityDecreased>(new InvalidOperationException("Item not in cart"));
    }
    return Result.FromValue(new ItemQuantityDecreased(current.Id, command.ProductId));
  }
}

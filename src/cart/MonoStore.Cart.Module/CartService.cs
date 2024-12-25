using DotNext;
using MonoStore.Cart.Contracts.Commands;
using MonoStore.Cart.Contracts.Dtos;
using MonoStore.Cart.Contracts.Requests;

namespace MonoStore.Cart.Module;

internal static class CartService
{
  private static bool IsItemInCart(Cart cart, string productId) =>
      cart.Items.Any(item => item.Product.Id == productId);

  private static bool IsCartArchived(Cart cart) =>
      cart.Status == CartStatus.Archived;

  private static bool IsCartOpen(Cart cart) =>
      cart.Status == CartStatus.Open;

  private static bool IsCartAbandoned(Cart cart) =>
      cart.Status == CartStatus.Abandoned;

  internal static Result<CartCreated> Create(CreateCart command)
  {
    // Todo: check that cart does not already exist
    return Result.FromValue(new CartCreated(command.CartId, command.OperatingChain));
  }

  internal static Result<ItemAddedToCart> Handle(Cart current, AddItem command)
  {
    if (!IsCartOpen(current))
    {
      return Result.FromException<ItemAddedToCart>(new InvalidCartStatusException(command.GetType().Name, CartStatus.Open, current.Status));
    }
    if (IsItemInCart(current, command.Item.Product.Id))
    {
      return Result.FromException<ItemAddedToCart>(new InvalidOperationException("Item already in cart"));
    }
    return Result.FromValue(new ItemAddedToCart(current.Id, command.Item));
  }

  internal static Result<ItemRemovedFromCart> Handle(Cart current, RemoveItem command)
  {
    if (!IsCartOpen(current))
    {
      return Result.FromException<ItemRemovedFromCart>(new InvalidCartStatusException(command.GetType().Name, CartStatus.Open, current.Status));
    }
    if (!IsItemInCart(current, command.ProductId))
    {
      return Result.FromException<ItemRemovedFromCart>(new InvalidOperationException("Item not in cart"));
    }
    return Result.FromValue(new ItemRemovedFromCart(current.Id, command.ProductId));
  }

  internal static Result<ItemQuantityIncreased> Handle(Cart current, IncreaseItemQuantity command)
  {
    if (!IsCartOpen(current))
    {
      return Result.FromException<ItemQuantityIncreased>(new InvalidCartStatusException(command.GetType().Name, CartStatus.Open, current.Status));
    }
    if (!IsItemInCart(current, command.ProductId))
    {
      return Result.FromException<ItemQuantityIncreased>(new InvalidOperationException("Item not in cart"));
    }
    return Result.FromValue(new ItemQuantityIncreased(current.Id, command.ProductId));
  }

  internal static Result<ItemQuantityDecreased> Handle(Cart current, DecreaseItemQuantity command)
  {
    if (!IsCartOpen(current))
    {
      return Result.FromException<ItemQuantityDecreased>(new InvalidCartStatusException(command.GetType().Name, CartStatus.Open, current.Status));
    }
    if (!IsItemInCart(current, command.ProductId))
    {
      return Result.FromException<ItemQuantityDecreased>(new InvalidOperationException("Item not in cart"));
    }
    return Result.FromValue(new ItemQuantityDecreased(current.Id, command.ProductId));
  }
  internal static Result<CartAbandoned> Handle(Cart current, AbandonCart command)
  {
    if (!IsCartOpen(current))
    {
      return Result.FromException<CartAbandoned>(new InvalidCartStatusException(command.GetType().Name, CartStatus.Open, current.Status));
    }
    return Result.FromValue(new CartAbandoned(current.Id));
  }

  internal static Result<CartRecovered> Handle(Cart current, RecoverCart command)
  {
    if (!IsCartAbandoned(current))
    {
      return Result.FromException<CartRecovered>(new InvalidCartStatusException(command.GetType().Name, CartStatus.Abandoned, current.Status));
    }
    return Result.FromValue(new CartRecovered(current.Id));
  }

  internal static Result<CartArchived> Handle(Cart current, ArchiveCart command)
  {
    if (IsCartArchived(current))
    {
      return Result.FromException<CartArchived>(new InvalidCartStatusException(command.GetType().Name, CartStatus.Open, current.Status));
    }
    return Result.FromValue(new CartArchived(current.Id));
  }

  internal static Result<CartCleared> Handle(Cart current, ClearCart command)
  {
    if (!IsCartOpen(current))
    {
      return Result.FromException<CartCleared>(new InvalidCartStatusException(command.GetType().Name, CartStatus.Open, current.Status));
    }
    return Result.FromValue(new CartCleared(current.Id));
  }
}

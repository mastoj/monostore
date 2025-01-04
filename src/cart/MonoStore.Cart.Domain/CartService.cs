using DotNext;
using MonoStore.Cart.Contracts.Commands;
using MonoStore.Cart.Contracts.Dtos;
using MonoStore.Cart.Contracts.Exceptions;
using MonoStore.Cart.Contracts.Requests;

namespace MonoStore.Cart.Domain;

public static class CartService
{
  private static bool IsItemInCart(Cart cart, string productId) =>
      cart.Items.Any(item => item.Product.Id == productId);

  private static bool IsCartArchived(Cart cart) =>
      cart.Status == CartStatus.Archived;

  private static bool IsCartOpen(Cart cart) =>
      cart.Status == CartStatus.Open;

  private static bool IsCartAbandoned(Cart cart) =>
      cart.Status == CartStatus.Abandoned;

  public static Result<CartCreated> Create(CreateCartMessage command)
  {
    // Todo: check that cart does not already exist
    return Result.FromValue(new CartCreated(command.CartId, command.OperatingChain, command.SessionId, command.UserId));
  }

  public static Result<ItemAddedToCart> Handle(Cart current, AddItem command)
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

  public static Result<ItemRemovedFromCart> Handle(Cart current, RemoveItem command)
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

  public static Result<ItemQuantityChanged> Handle(Cart current, ChangeItemQuantity command)
  {
    if (!IsCartOpen(current))
    {
      return Result.FromException<ItemQuantityChanged>(new InvalidCartStatusException(command.GetType().Name, CartStatus.Open, current.Status));
    }
    if (!IsItemInCart(current, command.ProductId))
    {
      return Result.FromException<ItemQuantityChanged>(new InvalidOperationException("Item not in cart"));
    }
    if (command.Quantity <= 0)
    {
      return Result.FromException<ItemQuantityChanged>(new InvalidOperationException("Quantity must be greater than 0"));
    }
    var item = current.Items.First(i => i.Product.Id == command.ProductId);
    if (item.Quantity == command.Quantity)
    {
      return Result.FromException<ItemQuantityChanged>(new InvalidOperationException("Quantity is the same"));
    }
    return Result.FromValue(new ItemQuantityChanged(current.Id, command.ProductId, command.Quantity));
  }

  public static Result<CartAbandoned> Handle(Cart current, AbandonCart command)
  {
    if (!IsCartOpen(current))
    {
      return Result.FromException<CartAbandoned>(new InvalidCartStatusException(command.GetType().Name, CartStatus.Open, current.Status));
    }
    return Result.FromValue(new CartAbandoned(current.Id));
  }

  public static Result<CartRecovered> Handle(Cart current, RecoverCart command)
  {
    if (!IsCartAbandoned(current))
    {
      return Result.FromException<CartRecovered>(new InvalidCartStatusException(command.GetType().Name, CartStatus.Abandoned, current.Status));
    }
    return Result.FromValue(new CartRecovered(current.Id));
  }

  public static Result<CartArchived> Handle(Cart current, ArchiveCart command)
  {
    if (IsCartArchived(current))
    {
      return Result.FromException<CartArchived>(new InvalidCartStatusException(command.GetType().Name, CartStatus.Open, current.Status));
    }
    return Result.FromValue(new CartArchived(current.Id));
  }

  public static Result<CartCleared> Handle(Cart current, ClearCart command)
  {
    if (!IsCartOpen(current))
    {
      return Result.FromException<CartCleared>(new InvalidCartStatusException(command.GetType().Name, CartStatus.Open, current.Status));
    }
    return Result.FromValue(new CartCleared(current.Id));
  }
}

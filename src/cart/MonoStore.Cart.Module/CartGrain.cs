
using Microsoft.Extensions.Logging;
using Monostore.ServiceDefaults;
using MonoStore.Cart.Contracts;
using MonoStore.Cart.Contracts.Grains;
using MonoStore.Product.Contracts;
using static MonoStore.Cart.Module.CartService;

namespace MonoStore.Cart.Module;

public interface IEventStore
{
  Task<TState> CreateStream<T, TState>(Guid id, T @event, Func<T, TState> apply, CancellationToken ct) where T : class;
  Task<TState> AppendToStream<T, TState>(Guid id, T @event, int version, Func<T, TState> apply, CancellationToken ct) where T : class;
  Task<TState> GetState<TState>(Guid id, CancellationToken ct) where TState : class;
}

internal static class Mappers
{
  internal static Contracts.Cart AsContract(this Cart cart)
  {
    return new Contracts.Cart
    {
      Id = cart.Id,
      OperatingChain = cart.OperatingChain,
      Status = cart.Status,
      Items = cart.Items.ToList(),
    };
  }
}

public sealed class CartGrain
  : Grain, ICartGrain
{
  private IEventStore eventStore;
  private readonly ILogger<CartGrain> logger;
  private Cart currentCart;

  public CartGrain(IEventStore eventStore, ILogger<CartGrain> logger)
  {
    this.eventStore = eventStore;
    this.logger = logger;
  }

  public override async Task OnActivateAsync(CancellationToken cancellationToken)
  {
    DiagnosticConfig.CartHost.ActiveCartCounter.Add(1, new KeyValuePair<string, object?>("operatingChain", "OCNOELK"));
    logger.LogInformation("Activating {grainKey}", this.GetPrimaryKeyString());
    var id = Guid.Parse(this.GetPrimaryKeyString().Split("/")[1]);
    currentCart = await eventStore.GetState<Cart>(id, default);
    await base.OnActivateAsync(cancellationToken);
  }

  public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
  {
    logger.LogInformation("Deactivating {grainKey}", this.GetPrimaryKeyString());
    DiagnosticConfig.CartHost.ActiveCartCounter.Add(-1, new KeyValuePair<string, object?>("operatingChain", "OCNOELK"));

    return base.OnDeactivateAsync(reason, cancellationToken);
  }

  public async Task<Contracts.Cart> CreateCart(CreateCart createCart)
  {
    var result = Create(createCart);
    if (result.IsSuccessful)
    {
      currentCart = await eventStore.CreateStream(createCart.CartId, result.Value, Cart.Create, default);
    }
    Console.WriteLine($"Created cart {createCart.CartId}: " + currentCart);
    return currentCart.AsContract();
  }

  public async Task<Contracts.Cart> AddItem(AddItem addItem)
  {
    Console.WriteLine($"Adding item {addItem.Item.Product.Id} to cart {addItem.CartId}");
    var result = Handle(currentCart, addItem);
    if (result.IsSuccessful)
    {
      currentCart = await eventStore.AppendToStream(addItem.CartId, result.Value, 1, currentCart.Apply, default);
    }
    Console.WriteLine($"Added item {addItem.Item.Product.Id} to cart {addItem.CartId}: " + currentCart);
    return currentCart.AsContract();
  }

  public async Task<Contracts.Cart> RemoveItem(RemoveItem removeItem)
  {
    var result = Handle(currentCart, removeItem);
    if (result.IsSuccessful)
    {
      currentCart = await eventStore.AppendToStream(removeItem.CartId, result.Value, 2, currentCart.Apply, default);
    }
    Console.WriteLine($"Removed item {removeItem.ProductId} from cart {removeItem.CartId}: " + currentCart);
    return currentCart.AsContract();
  }

  public async Task<Contracts.Cart> IncreaseItemQuantity(IncreaseItemQuantity increaseItemQuantity)
  {
    var result = Handle(currentCart, increaseItemQuantity);
    if (result.IsSuccessful)
    {
      currentCart = await eventStore.AppendToStream(increaseItemQuantity.CartId, result.Value, 2, currentCart.Apply, default);
    }
    logger.LogInformation("Increased item {productId} quantity in cart {cartId}", increaseItemQuantity.ProductId, increaseItemQuantity.CartId);
    return currentCart.AsContract();
  }

  public async Task<Contracts.Cart> DecreaseItemQuantity(DecreaseItemQuantity decreaseItemQuantity)
  {
    var result = Handle(currentCart, decreaseItemQuantity);
    if (result.IsSuccessful)
    {
      currentCart = await eventStore.AppendToStream(decreaseItemQuantity.CartId, result.Value, 2, currentCart.Apply, default);
    }
    Console.WriteLine($"Decreased item {decreaseItemQuantity.ProductId} quantity in cart {decreaseItemQuantity.CartId}: " + currentCart);
    return currentCart.AsContract();
  }

  #region queries
  public Task<Contracts.Cart> GetCart(GetCart getCart)
  {
    if (getCart.CartId == Guid.Empty)
    {
      throw new ArgumentException("CartId cannot be empty", nameof(getCart));
    }
    if (getCart.CartId != currentCart.Id)
    {
      throw new InvalidOperationException("Cart not found");
    }
    return Task.FromResult(currentCart.AsContract());
  }

  public async Task<Contracts.Cart> ClearCart(ClearCart clearCart)
  {
    var result = Handle(currentCart, clearCart);
    if (result.IsSuccessful)
    {
      currentCart = await eventStore.AppendToStream(clearCart.CartId, result.Value, 2, currentCart.Apply, default);
    }
    Console.WriteLine($"Cleared cart {clearCart.CartId}: " + currentCart);
    return currentCart.AsContract();
  }

  public async Task<Contracts.Cart> AbandonCart(AbandonCart abandonCart)
  {
    var result = Handle(currentCart, abandonCart);
    if (result.IsSuccessful)
    {
      currentCart = await eventStore.AppendToStream(abandonCart.CartId, result.Value, 2, currentCart.Apply, default);
    }
    Console.WriteLine($"Abandoned cart {abandonCart.CartId}: " + currentCart);
    return currentCart.AsContract();
  }

  public async Task<Contracts.Cart> RecoverCart(RecoverCart recoverCart)
  {
    var result = Handle(currentCart, recoverCart);
    if (result.IsSuccessful)
    {
      currentCart = await eventStore.AppendToStream(recoverCart.CartId, result.Value, 2, currentCart.Apply, default);
    }
    Console.WriteLine($"Recovered cart {recoverCart.CartId}: " + currentCart);
    return currentCart.AsContract();
  }

  public async Task<Contracts.Cart> ArchiveCart(ArchiveCart archiveCart)
  {
    var result = Handle(currentCart, archiveCart);
    if (result.IsSuccessful)
    {
      currentCart = await eventStore.AppendToStream(archiveCart.CartId, result.Value, 2, currentCart.Apply, default);
    }
    Console.WriteLine($"Archived cart {archiveCart.CartId}: " + currentCart);
    return currentCart.AsContract();
  }
  #endregion
}

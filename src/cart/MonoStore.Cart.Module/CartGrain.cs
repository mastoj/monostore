
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
  // internal static Contracts.CartStatus AsContract(this CartStatus status)
  // {
  //   return status switch
  //   {
  //     CartStatus.Open => Contracts.CartStatus.Open,
  //     CartStatus.TimedOut => Contracts.CartStatus.TimedOut,
  //     CartStatus.Paid => Contracts.CartStatus.Paid,
  //     _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
  //   };
  // }

  // internal static Contracts.Product AsContract(this Product product)
  // {
  //   return new Contracts.Product
  //   {
  //     Id = product.ProductId,
  //     Name = product.Name,
  //     Price = product.Price,
  //     PriceExVat = product.PriceExVat
  //   };
  // }

  // internal static CartItem AsContract(this CartItem item)
  // {
  //   return new CartItem
  //   {
  //     Product = item.Product.AsContract(),
  //     Quantity = item.Quantity
  //   };
  // }

  internal static Contracts.Cart AsContract(this Cart cart)
  {
    return new Contracts.Cart
    {
      Id = cart.Id,
      OperatingChain = cart.OperatingChain,
      Status = cart.Status, //cart.Status.AsContract(),
      Items = cart.Items.ToList(), // cart.Items.Select(i => i.AsContract()).ToList()
    };
  }
}

public sealed class CartGrain
  : Grain, ICartGrain
{
  private IEventStore eventStore;
  // private readonly IProductService productService;
  private readonly ILogger<CartGrain> logger;
  private Cart currentCart;
  // private IGrainFactory grainFactory;

  public CartGrain(IEventStore eventStore, ILogger<CartGrain> logger)
  {
    this.eventStore = eventStore;
    //    this.productService = productService;
    this.logger = logger;
    // this.grainFactory = grainFactory;
  }

  public override async Task OnActivateAsync(CancellationToken cancellationToken)
  {
    //    DelayDeactivation(TimeSpan.FromMinutes(10));
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
  #endregion
}

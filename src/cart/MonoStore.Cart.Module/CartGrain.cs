
using DotNext;
using Microsoft.Extensions.Logging;
using MonoStore.Product.Contracts;
using static MonoStore.Cart.Module.CartService;

namespace MonoStore.Cart.Module;

public interface ICartGrain : IGrainWithStringKey
{
  Task<Contracts.Cart> CreateCart(Guid id);
  Task<Contracts.Cart> GetCart(Guid id);
  Task<Contracts.Cart> AddItem(Contracts.AddItem addItem);
  Task<Contracts.Cart> RemoveItem(Contracts.RemoveItem removeItem);
  Task<Contracts.Cart> IncreaseItemQuantity(Contracts.IncreaseItemQuantity increaseItemQuantity);
  Task<Contracts.Cart> DecreaseItemQuantity(Contracts.DecreaseItemQuantity decreaseItemQuantity);
}


public interface IEventStore
{
  Task<TState> CreateStream<T, TState>(Guid id, T @event, Func<T, TState> apply, CancellationToken ct) where T : class;
  Task<TState> AppendToStream<T, TState>(Guid id, T @event, int version, Func<T, TState> apply, CancellationToken ct) where T : class;
  Task<TState> GetState<TState>(Guid id, CancellationToken ct) where TState : class;
}

internal static class Mappers
{
  internal static Contracts.CartStatus AsContract(this CartStatus status)
  {
    return status switch
    {
      CartStatus.Open => Contracts.CartStatus.Open,
      CartStatus.TimedOut => Contracts.CartStatus.TimedOut,
      CartStatus.Paid => Contracts.CartStatus.Paid,
      _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
    };
  }

  internal static Contracts.Product AsContract(this Product product)
  {
    return new Contracts.Product
    {
      Id = product.ProductId,
      Name = product.Name,
      Price = product.Price,
      PriceExVat = product.PriceExVat
    };
  }

  internal static Contracts.CartItem AsContract(this CartItem item)
  {
    return new Contracts.CartItem
    {
      Product = item.Product.AsContract(),
      Quantity = item.Quantity
    };
  }

  internal static Contracts.Cart AsContract(this Cart cart)
  {
    return new Contracts.Cart
    {
      Id = cart.Id.ToString(),
      Status = cart.Status.AsContract(),
      Items = cart.Items.Select(i => i.AsContract()).ToList()
    };
  }
}

public sealed class CartGrain
  : Grain, ICartGrain
{
  private IEventStore eventStore;
  private readonly IProductService productService;
  private readonly ILogger<CartGrain> logger;
  private Cart currentCart;

  public CartGrain(IEventStore eventStore, IProductService productService, ILogger<CartGrain> logger)
  {
    this.eventStore = eventStore;
    this.productService = productService;
    this.logger = logger;
  }

  public override async Task OnActivateAsync(CancellationToken cancellationToken)
  {
    //    DelayDeactivation(TimeSpan.FromMinutes(10));
    logger.LogInformation("Activating {grainKey}", this.GetPrimaryKeyString());
    var id = Guid.Parse(this.GetPrimaryKeyString().Split("/")[1]);
    currentCart = await eventStore.GetState<Cart>(id, default);
    await base.OnActivateAsync(cancellationToken);
  }

  public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
  {
    logger.LogInformation("Deactivating {grainKey}", this.GetPrimaryKeyString());
    return base.OnDeactivateAsync(reason, cancellationToken);
  }

  public async Task<Contracts.Cart> CreateCart(Guid id)
  {
    var cartId = id;
    var result = Create(new CreateCart(cartId));
    if (result.IsSuccessful)
    {
      currentCart = await eventStore.CreateStream(cartId, result.Value, Cart.Create, default);
    }
    Console.WriteLine($"Created cart {id}: " + currentCart);
    return currentCart.AsContract();
  }

  public async Task<Contracts.Cart> AddItem(Contracts.AddItem addItem)
  {
    var productDto = await productService.GetProductAsync(addItem.ProductId, "OCNOELK");
    // Should get product from product facade
    var product = new Product(addItem.ProductId, "Name: " + addItem.ProductId, 100, 100);
    var result = Handle(currentCart, new AddItem(Guid.Parse(addItem.CartId), product));
    if (result.IsSuccessful)
    {
      currentCart = await eventStore.AppendToStream(Guid.Parse(addItem.CartId), result.Value, 1, currentCart.Apply, default);
    }
    Console.WriteLine($"Added item {addItem.ProductId} to cart {addItem.CartId}: " + currentCart);
    return currentCart.AsContract();
  }

  public async Task<Contracts.Cart> RemoveItem(Contracts.RemoveItem removeItem)
  {
    var result = Handle(currentCart, new RemoveItem(Guid.Parse(removeItem.CartId), removeItem.ProductId));
    if (result.IsSuccessful)
    {
      currentCart = await eventStore.AppendToStream(Guid.Parse(removeItem.CartId), result.Value, 2, currentCart.Apply, default);
    }
    Console.WriteLine($"Removed item {removeItem.ProductId} from cart {removeItem.CartId}: " + currentCart);
    return currentCart.AsContract();
  }

  public async Task<Contracts.Cart> IncreaseItemQuantity(Contracts.IncreaseItemQuantity increaseItemQuantity)
  {
    var result = Handle(currentCart, new IncreaseItemQuantity(Guid.Parse(increaseItemQuantity.CartId), increaseItemQuantity.ProductId));
    if (result.IsSuccessful)
    {
      currentCart = await eventStore.AppendToStream(Guid.Parse(increaseItemQuantity.CartId), result.Value, 2, currentCart.Apply, default);
    }
    logger.LogInformation("Increased item {productId} quantity in cart {cartId}", increaseItemQuantity.ProductId, increaseItemQuantity.CartId);
    return currentCart.AsContract();
  }

  public async Task<Contracts.Cart> DecreaseItemQuantity(Contracts.DecreaseItemQuantity decreaseItemQuantity)
  {
    var result = Handle(currentCart, new DecreaseItemQuantity(Guid.Parse(decreaseItemQuantity.CartId), decreaseItemQuantity.ProductId));
    if (result.IsSuccessful)
    {
      currentCart = await eventStore.AppendToStream(Guid.Parse(decreaseItemQuantity.CartId), result.Value, 2, currentCart.Apply, default);
    }
    Console.WriteLine($"Decreased item {decreaseItemQuantity.ProductId} quantity in cart {decreaseItemQuantity.CartId}: " + currentCart);
    return currentCart.AsContract();
  }

  public Task<Contracts.Cart> GetCart(Guid id)
  {
    return Task.FromResult(currentCart.AsContract());
  }
}

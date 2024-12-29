
using Microsoft.Extensions.Logging;
using Monostore.ServiceDefaults;
using MonoStore.Cart.Contracts.Commands;
using MonoStore.Cart.Contracts.Dtos;
using MonoStore.Cart.Contracts.Grains;
using MonoStore.Cart.Contracts.Requests;
using MonoStore.Product.Contracts.Grains;
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
  internal static CartData AsContract(this Cart cart)
  {
    return new CartData(cart.Id, cart.Version, cart.OperatingChain, cart.Status, cart.Items.ToList());
  }
}

public sealed class CartGrain
  : Grain, ICartGrain
{
  private IEventStore eventStore;
  private readonly ILogger<CartGrain> logger;
  private readonly IGrainFactory grains;
  private Cart currentCart;

  public CartGrain(IEventStore eventStore, IGrainFactory grains, ILogger<CartGrain> logger)
  {
    this.eventStore = eventStore;
    this.logger = logger;
    this.grains = grains;
    currentCart = new Cart(Guid.Empty, "", CartStatus.Open, new List<CartItem>());
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

  public async Task<GrainResult<CartData, CartError>> CreateCart(CreateCart createCart)
  {
    var result = Create(createCart);
    if (result.IsSuccessful)
    {
      currentCart = await eventStore.CreateStream(createCart.CartId, result.Value, Cart.Create, default);
    }
    Console.WriteLine($"Created cart {currentCart.Id}: " + currentCart);
    return GrainResult<CartData, CartError>.Success(currentCart.AsContract());
  }

  public async Task<GrainResult<CartData, CartError>> AddItem(AddItemRequest addItemRequest)
  {
    var productGrainId = IProductGrain.ProductGrainId(addItemRequest.OperatingChain, addItemRequest.ProductId);
    var productGrain = grains.GetGrain<IProductGrain>(productGrainId);
    var product = await productGrain.GetProductAsync();
    if (product == null)
    {
      throw new Exception("Product not found");
    }
    if (product.Price == null)
    {
      throw new Exception("Product price not found");
    }
    var addItem = new AddItem(new CartItem(
      new Contracts.Dtos.Product(product.ArticleNumber, product.Name ?? "", product.Price.Price, product.Price.PriceExclVat, product.ProductUrl ?? "", product.ImageUrl ?? ""),
      1
    ));

    var result = Handle(currentCart, addItem);
    if (result.IsSuccessful)
    {
      currentCart = await eventStore.AppendToStream(currentCart.Id, result.Value, currentCart.Version, currentCart.Apply, default);
    }
    return GrainResult<CartData, CartError>.Success(currentCart.AsContract());
  }

  public async Task<GrainResult<CartData, CartError>> RemoveItem(RemoveItem removeItem)
  {
    var result = Handle(currentCart, removeItem);
    if (result.IsSuccessful)
    {
      currentCart = await eventStore.AppendToStream(currentCart.Id, result.Value, currentCart.Version, currentCart.Apply, default);
    }
    Console.WriteLine($"Removed item {removeItem.ProductId} from cart {currentCart.Id}: " + currentCart);
    return GrainResult<CartData, CartError>.Success(currentCart.AsContract());
  }

  public async Task<GrainResult<CartData, CartError>> ChangeItemQuantity(ChangeItemQuantity changeItemQuantity)
  {
    var result = Handle(currentCart, changeItemQuantity);
    if (result.IsSuccessful)
    {
      currentCart = await eventStore.AppendToStream(currentCart.Id, result.Value, currentCart.Version, currentCart.Apply, default);
    }
    logger.LogInformation("Increased item {productId} quantity in cart {cartId}", changeItemQuantity.ProductId, currentCart.Id);
    return GrainResult<CartData, CartError>.Success(currentCart.AsContract());
  }

  #region queries
  public Task<GrainResult<CartData, CartError>> GetCart(GetCart getCart)
  {
    if (currentCart.Id == Guid.Empty)
    {
      throw new ArgumentException("CartId cannot be empty", nameof(getCart));
    }
    if (currentCart.Id != currentCart.Id)
    {
      throw new InvalidOperationException("Cart not found");
    }
    return Task.FromResult(GrainResult<CartData, CartError>.Success(currentCart.AsContract()));
  }

  public async Task<GrainResult<CartData, CartError>> ClearCart(ClearCart clearCart)
  {
    var result = Handle(currentCart, clearCart);
    if (result.IsSuccessful)
    {
      currentCart = await eventStore.AppendToStream(currentCart.Id, result.Value, currentCart.Version, currentCart.Apply, default);
    }
    Console.WriteLine($"Cleared cart {currentCart.Id}: " + currentCart);
    return GrainResult<CartData, CartError>.Success(currentCart.AsContract());
  }

  public async Task<GrainResult<CartData, CartError>> AbandonCart(AbandonCart abandonCart)
  {
    var result = Handle(currentCart, abandonCart);
    if (result.IsSuccessful)
    {
      currentCart = await eventStore.AppendToStream(currentCart.Id, result.Value, currentCart.Version, currentCart.Apply, default);
    }
    Console.WriteLine($"Abandoned cart {currentCart.Id}: " + currentCart);
    return GrainResult<CartData, CartError>.Success(currentCart.AsContract());
  }

  public async Task<GrainResult<CartData, CartError>> RecoverCart(RecoverCart recoverCart)
  {
    var result = Handle(currentCart, recoverCart);
    if (result.IsSuccessful)
    {
      currentCart = await eventStore.AppendToStream(currentCart.Id, result.Value, currentCart.Version, currentCart.Apply, default);
    }
    Console.WriteLine($"Recovered cart {currentCart.Id}: " + currentCart);
    return GrainResult<CartData, CartError>.Success(currentCart.AsContract());
  }

  public async Task<GrainResult<CartData, CartError>> ArchiveCart(ArchiveCart archiveCart)
  {
    var result = Handle(currentCart, archiveCart);
    if (result.IsSuccessful)
    {
      currentCart = await eventStore.AppendToStream(currentCart.Id, result.Value, currentCart.Version, currentCart.Apply, default);
    }
    Console.WriteLine($"Archived cart {currentCart.Id}: " + currentCart);
    return GrainResult<CartData, CartError>.Success(currentCart.AsContract());
  }
  #endregion
}

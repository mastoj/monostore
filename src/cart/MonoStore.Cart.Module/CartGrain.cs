using Monostore.Orleans.Types;
using Monostore.ServiceDefaults;
using MonoStore.Contracts.Cart.Commands;
using MonoStore.Contracts.Cart.Dtos;
using MonoStore.Contracts.Cart.Grains;
using MonoStore.Contracts.Cart.Requests;
using MonoStore.Marten;
using MonoStore.Contracts.Product.Grains;
using static MonoStore.Cart.Domain.CartService;

namespace MonoStore.Cart.Module;

internal static class Mappers
{
  internal static CartData AsContract(this Domain.Cart cart)
  {
    // var totals = cart.Items.Aggregate((total: 0m, totalExVat: 0m, beforePrice: 0m, beforePriceExVat: 0m), (acc, item) =>
    // {
    //   var price = item.Product?.Price ?? 0;
    //   var priceExVat = item.Product?.PriceExVat ?? 0;
    //   var beforePrice = item.Product?.BeforePrice ?? 0;
    //   var beforePriceExVat = item.Product?.BeforePriceExVat ?? 0;
    //   return (acc.total + price * item.Quantity, acc.totalExVat + priceExVat * item.Quantity, acc.beforePrice + beforePrice * item.Quantity, acc.beforePriceExVat + beforePriceExVat * item.Quantity);
    // });
    return new CartData(cart.Id, cart.Version, cart.OperatingChain, cart.Status, cart.Items.ToList(), cart.Total, cart.TotalExVat, cart.BeforePriceTotal, cart.BeforePriceExVatTotal, cart.SessionId, cart.UserId);
  }
}

public sealed class CartGrain
  : Grain, ICartGrain
{
  private IEventStore eventStore;
  private readonly ILogger<CartGrain> logger;
  private readonly IGrainFactory grains;
  private Domain.Cart? _currentCart;
  private Domain.Cart CurrentCart
  {
    get => _currentCart ?? throw new InvalidOperationException("Cart not found");
    set
    {
      _currentCart = value;
      logger.LogInformation("Cart {cartId} updated to version {version}", value.Id, value.Version);
    }
  }

  public CartGrain(IEventStore eventStore, IGrainFactory grains, ILogger<CartGrain> logger)
  {
    this.eventStore = eventStore;
    this.logger = logger;
    this.grains = grains;
  }

  public override async Task OnActivateAsync(CancellationToken cancellationToken)
  {
    DiagnosticConfig.CartHost.ActiveCartCounter.Add(1, new KeyValuePair<string, object?>("operatingChain", "OCNOELK"));
    logger.LogInformation("Activating {grainKey}", this.GetPrimaryKeyString());
    var id = Guid.Parse(this.GetPrimaryKeyString().Split("/")[1]);
    _currentCart = await eventStore.GetState<Domain.Cart>(id, default);
    await base.OnActivateAsync(cancellationToken);
  }

  public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
  {
    logger.LogInformation("Deactivating {grainKey}", this.GetPrimaryKeyString());
    DiagnosticConfig.CartHost.ActiveCartCounter.Add(-1, new KeyValuePair<string, object?>("operatingChain", "OCNOELK"));

    return base.OnDeactivateAsync(reason, cancellationToken);
  }

  public async Task<GrainResult<CartData, CartError>> CreateCart(CreateCartMessage createCart)
  {
    var result = Create(createCart);
    if (result.IsSuccessful)
    {
      CurrentCart = await eventStore.CreateStream(createCart.CartId, result.Value, Domain.Cart.Create, default);
    }
    return GrainResult<CartData, CartError>.Success(CurrentCart.AsContract());
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
      new Product(product.ArticleNumber, product.Name ?? "", product.Price.Price, product.Price.PriceExclVat, product.BeforePrice?.Price, product.BeforePrice?.PriceExclVat, product.ProductUrl ?? "", product.ImageUrl ?? ""),
      1
    ));

    var result = Handle(CurrentCart, addItem);
    if (result.IsSuccessful)
    {
      CurrentCart = await eventStore.AppendToStream(CurrentCart.Id, result.Value, CurrentCart.Version, CurrentCart.Apply, default);
    }
    return GrainResult<CartData, CartError>.Success(CurrentCart.AsContract());
  }

  public async Task<GrainResult<CartData, CartError>> RemoveItem(RemoveItem removeItem)
  {
    var result = Handle(CurrentCart, removeItem);
    if (result.IsSuccessful)
    {
      CurrentCart = await eventStore.AppendToStream(CurrentCart.Id, result.Value, CurrentCart.Version, CurrentCart.Apply, default);
    }
    Console.WriteLine($"Removed item {removeItem.ProductId} from cart {CurrentCart.Id}: " + CurrentCart);
    return GrainResult<CartData, CartError>.Success(CurrentCart.AsContract());
  }

  public async Task<GrainResult<CartData, CartError>> ChangeItemQuantity(ChangeItemQuantity changeItemQuantity)
  {
    var result = Handle(CurrentCart, changeItemQuantity);
    if (result.IsSuccessful)
    {
      CurrentCart = await eventStore.AppendToStream(CurrentCart.Id, result.Value, CurrentCart.Version, CurrentCart.Apply, default);
    }
    logger.LogInformation("Increased item {productId} quantity in cart {cartId}", changeItemQuantity.ProductId, CurrentCart.Id);
    return GrainResult<CartData, CartError>.Success(CurrentCart.AsContract());
  }

  #region queries
  public Task<GrainResult<CartData, CartError>> GetCart(GetCart getCart)
  {
    if (CurrentCart.Id == Guid.Empty)
    {
      throw new ArgumentException("CartId cannot be empty", nameof(getCart));
    }
    if (CurrentCart.Id != CurrentCart.Id)
    {
      throw new InvalidOperationException("Cart not found");
    }
    return Task.FromResult(GrainResult<CartData, CartError>.Success(CurrentCart.AsContract()));
  }

  public async Task<GrainResult<CartData, CartError>> ClearCart(ClearCart clearCart)
  {
    var result = Handle(CurrentCart, clearCart);
    if (result.IsSuccessful)
    {
      CurrentCart = await eventStore.AppendToStream(CurrentCart.Id, result.Value, CurrentCart.Version, CurrentCart.Apply, default);
    }
    Console.WriteLine($"Cleared cart {CurrentCart.Id}: " + CurrentCart);
    return GrainResult<CartData, CartError>.Success(CurrentCart.AsContract());
  }

  public async Task<GrainResult<CartData, CartError>> AbandonCart(AbandonCart abandonCart)
  {
    var result = Handle(CurrentCart, abandonCart);
    if (result.IsSuccessful)
    {
      CurrentCart = await eventStore.AppendToStream(CurrentCart.Id, result.Value, CurrentCart.Version, CurrentCart.Apply, default);
    }
    Console.WriteLine($"Abandoned cart {CurrentCart.Id}: " + CurrentCart);
    return GrainResult<CartData, CartError>.Success(CurrentCart.AsContract());
  }

  public async Task<GrainResult<CartData, CartError>> RecoverCart(RecoverCart recoverCart)
  {
    var result = Handle(CurrentCart, recoverCart);
    if (result.IsSuccessful)
    {
      CurrentCart = await eventStore.AppendToStream(CurrentCart.Id, result.Value, CurrentCart.Version, CurrentCart.Apply, default);
    }
    Console.WriteLine($"Recovered cart {CurrentCart.Id}: " + CurrentCart);
    return GrainResult<CartData, CartError>.Success(CurrentCart.AsContract());
  }

  public async Task<GrainResult<CartData, CartError>> ArchiveCart(ArchiveCart archiveCart)
  {
    var result = Handle(CurrentCart, archiveCart);
    if (result.IsSuccessful)
    {
      CurrentCart = await eventStore.AppendToStream(CurrentCart.Id, result.Value, CurrentCart.Version, CurrentCart.Apply, default);
    }
    Console.WriteLine($"Archived cart {CurrentCart.Id}: " + CurrentCart);
    return GrainResult<CartData, CartError>.Success(CurrentCart.AsContract());
  }
  #endregion
}

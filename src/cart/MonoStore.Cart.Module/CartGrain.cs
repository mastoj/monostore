
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
  private IEventStore _eventStore;
  private Cart _currentCart;

  public CartGrain(IEventStore eventStore)
  {
    _eventStore = eventStore;
  }

  public override async Task OnActivateAsync(CancellationToken cancellationToken)
  {
    //    DelayDeactivation(TimeSpan.FromMinutes(10));
    Console.WriteLine($"Activating! {this.GetPrimaryKeyString()}");
    var id = Guid.Parse(this.GetPrimaryKeyString().Split("/")[1]);
    Console.WriteLine($"Loading cart {id}");
    _currentCart = await _eventStore.GetState<Cart>(id, default);
    await base.OnActivateAsync(cancellationToken);
  }

  public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
  {
    Console.WriteLine($"Deactivating! {this.GetPrimaryKeyString()}");
    return base.OnDeactivateAsync(reason, cancellationToken);
  }

  public async Task<Contracts.Cart> CreateCart(Guid id)
  {
    var cartId = id;
    var result = Create(new CreateCart(cartId));
    if (result.IsSuccessful)
    {
      _currentCart = await _eventStore.CreateStream(cartId, result.Value, Cart.Create, default);
    }
    Console.WriteLine($"Created cart {id}: " + _currentCart);
    return _currentCart.AsContract();
  }

  public async Task<Contracts.Cart> AddItem(Contracts.AddItem addItem)
  {
    // Should get product from product facade
    var product = new Product(addItem.ProductId, "Name: " + addItem.ProductId, 100, 100);
    var result = Handle(_currentCart, new AddItem(Guid.Parse(addItem.CartId), product));
    if (result.IsSuccessful)
    {
      _currentCart = await _eventStore.AppendToStream(Guid.Parse(addItem.CartId), result.Value, 1, _currentCart.Apply, default);
    }
    Console.WriteLine($"Added item {addItem.ProductId} to cart {addItem.CartId}: " + _currentCart);
    return _currentCart.AsContract();
  }

  public async Task<Contracts.Cart> RemoveItem(Contracts.RemoveItem removeItem)
  {
    var result = Handle(_currentCart, new RemoveItem(Guid.Parse(removeItem.CartId), removeItem.ProductId));
    if (result.IsSuccessful)
    {
      _currentCart = await _eventStore.AppendToStream(Guid.Parse(removeItem.CartId), result.Value, 2, _currentCart.Apply, default);
    }
    Console.WriteLine($"Removed item {removeItem.ProductId} from cart {removeItem.CartId}: " + _currentCart);
    return _currentCart.AsContract();
  }

  public Task<Contracts.Cart> IncreaseItemQuantity(Contracts.IncreaseItemQuantity increaseItemQuantity)
  {
    throw new NotImplementedException();
  }

  public Task<Contracts.Cart> DecreaseItemQuantity(Contracts.DecreaseItemQuantity decreaseItemQuantity)
  {
    throw new NotImplementedException();
  }

  public Task<Contracts.Cart> GetCart(Guid id)
  {
    return Task.FromResult(_currentCart.AsContract());
  }
}

// [GenerateSerializer, Alias(nameof(CartDetails))]
// public sealed record class CartDetails
// {
//   [Id(0)]
//   public string Id { get; set; } = "";
// }

// [GenerateSerializer, Alias(nameof(AddItem))]
// public sealed record class AddItem
// {
//   [Id(0)]
//   public string cartId { get; set; } = "";
//   [Id(1)]
//   public string itemId { get; set; } = "";
// }

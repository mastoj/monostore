
using static MonoStore.Cart.Module.CartService;

namespace MonoStore.Cart.Module;

public interface ICartGrain : IGrainWithStringKey
{
  Task<Contracts.Cart> CreateCart(Guid id);
  Task<Contracts.Cart> AddItem(Contracts.AddItem addItem);
  Task<Contracts.Cart> RemoveItem(Contracts.RemoveItem removeItem);
  Task<Contracts.Cart> IncreaseItemQuantity(Contracts.IncreaseItemQuantity increaseItemQuantity);
  Task<Contracts.Cart> DecreaseItemQuantity(Contracts.DecreaseItemQuantity decreaseItemQuantity);
}


public interface IEventStore
{
  Task<T> CreateStream<T>(Guid id, T @event, CancellationToken ct);
  Task<T> AppendToStream<T>(Guid id, T @event, CancellationToken ct);
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

  public override Task OnActivateAsync(CancellationToken cancellationToken)
  {
    //    DelayDeactivation(TimeSpan.FromMinutes(10));
    Console.WriteLine($"Activating! {this.GetPrimaryKeyString()}");
    return base.OnActivateAsync(cancellationToken);
  }


  public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
  {
    Console.WriteLine($"Deactivating! {this.GetPrimaryKeyString()}");
    return base.OnDeactivateAsync(reason, cancellationToken);
  }
  public Task<Contracts.Cart> AddItem(Contracts.AddItem addItem)
  {
    // Should get product from product facade
    var product = new Product(addItem.ProductId, "Name: " + addItem.ProductId, 100, 100);
    var result = Handle(_currentCart, new AddItem(Guid.Parse(addItem.CartId), product));
    if (result.IsSuccessful)
    {
      _eventStore.AppendToStream(Guid.Parse(addItem.CartId), result.Value, default);
      _currentCart = _currentCart.Apply(result.Value);
    }
    Console.WriteLine($"Added item {addItem.ProductId} to cart {addItem.CartId}: " + _currentCart);
    return Task.FromResult(_currentCart.AsContract());
  }

  // public async Task<CartDetails> CreateCart(string id)
  // {
  //   Console.WriteLine($"Getting cart for {id} {state.State}");
  //   if (state.State.Id is "")
  //   {
  //     var newState = new CartDetails
  //     {
  //       Id = id
  //     };
  //     state.State = newState;
  //     Console.WriteLine($"Cart for {id} is new {newState.Id}");
  //   }
  //   await state.WriteStateAsync();
  //   Console.WriteLine($"Cart for {id} is {state.State.Id}");
  //   return state.State;
  // }

  // Task<Contracts.Cart> ICartGrain.CreateCart(string id)
  // {

  // }

  // public Task<Contracts.Cart> AddItem(Contracts.AddItem addItem)
  // {
  //   throw new NotImplementedException();
  // }

  public Task<Contracts.Cart> RemoveItem(Contracts.RemoveItem removeItem)
  {
    throw new NotImplementedException();
  }

  public Task<Contracts.Cart> IncreaseItemQuantity(Contracts.IncreaseItemQuantity increaseItemQuantity)
  {
    throw new NotImplementedException();
  }

  public Task<Contracts.Cart> DecreaseItemQuantity(Contracts.DecreaseItemQuantity decreaseItemQuantity)
  {
    throw new NotImplementedException();
  }

  public Task<Contracts.Cart> CreateCart(Guid id)
  {
    var cartId = id;
    var result = Create(new CreateCart(cartId));
    if (result.IsSuccessful)
    {
      _eventStore.CreateStream(cartId, result.Value, default);
      _currentCart = Cart.Create(result.Value);
    }
    Console.WriteLine($"Created cart {id}: " + _currentCart);
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

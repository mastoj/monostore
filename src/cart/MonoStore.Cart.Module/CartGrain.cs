namespace MonoStore.Cart.Module;

public interface ICartGrain : IGrainWithStringKey
{
  Task<CartDetails> GetCart(string id);
  Task<CartDetails> AddItem(AddItem addItem);
}

public sealed class CartGrain(
  [PersistentState(
        stateName: "cart",
        storageName: "carts")]
        IPersistentState<CartDetails> state)
  : Grain, ICartGrain
{
  public Task<CartDetails> AddItem(AddItem addItem)
  {
    throw new NotImplementedException();
  }

  public async Task<CartDetails> GetCart(string id)
  {
    Console.WriteLine($"Getting cart for {id} {state.State}");
    if (state.State.Id is "")
    {
      var newState = new CartDetails
      {
        Id = id
      };
      state.State = newState;
      Console.WriteLine($"Cart for {id} is new {newState.Id}");
    }
    await state.WriteStateAsync();
    Console.WriteLine($"Cart for {id} is {state.State.Id}");
    return state.State;
  }
}

[GenerateSerializer, Alias(nameof(CartDetails))]
public sealed record class CartDetails
{
  [Id(0)]
  public string Id { get; set; } = "";
}
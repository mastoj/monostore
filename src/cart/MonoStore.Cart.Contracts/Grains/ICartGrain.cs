namespace MonoStore.Cart.Contracts.Grains;

public interface ICartGrain : IGrainWithStringKey
{
  Task<Cart> CreateCart(CreateCart createCart);
  Task<Cart> GetCart(GetCart getCart);
  Task<Cart> AddItem(AddItem addItem);
  Task<Cart> RemoveItem(RemoveItem removeItem);
  Task<Cart> IncreaseItemQuantity(IncreaseItemQuantity increaseItemQuantity);
  Task<Cart> DecreaseItemQuantity(DecreaseItemQuantity decreaseItemQuantity);
}
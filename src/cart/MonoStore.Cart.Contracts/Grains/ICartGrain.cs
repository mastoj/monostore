namespace MonoStore.Cart.Contracts.Grains;

public interface ICartGrain : IGrainWithStringKey
{
  Task<Cart> CreateCart(Guid id);
  Task<Cart> GetCart(Guid id);
  Task<Cart> AddItem(AddItem addItem);
  Task<Cart> RemoveItem(RemoveItem removeItem);
  Task<Cart> IncreaseItemQuantity(IncreaseItemQuantity increaseItemQuantity);
  Task<Cart> DecreaseItemQuantity(DecreaseItemQuantity decreaseItemQuantity);
}
namespace MonoStore.Cart.Contracts;

public record AddItem(string cartId, string itemId);
public record RemoveItem(string cartId, string itemId);
public record IncreaseItemQuantity(string cartId, string itemId);
public record DecreaseItemQuantity(string cartId, string itemId);
public record Cart(string id, List<CartItem> items, decimal total, decimal totalExVat);
public record CartItem(string id, string name, int quantity, decimal price, decimal priceExVat);
namespace MonoStore.Cart.Contracts.Dtos;

public enum CartStatus
{
  Open,
  TimedOut,
  Abandoned,
  Archived,
}

[GenerateSerializer]
public record Product(string Id, string Name, decimal Price, decimal PriceExVat, string Url, string PrimaryImageUrl);

[GenerateSerializer]
public record CartItem(Product Product, int Quantity);

[GenerateSerializer]
public record class CartData(Guid Id, int Version, string OperatingChain, CartStatus Status, List<CartItem> Items, string sessionId, string? userId);

namespace MonoStore.Checkout.Contracts;

[GenerateSerializer]
public record Product(string Id, string Name, decimal Price, decimal PriceExVat, string Url, string PrimaryImageUrl);
[GenerateSerializer]
public record PurchaseOrderItem(Product Product, int Quantity);
[GenerateSerializer]
public record PurchaseOrderData(Guid PurchaseOrderId, int Version, List<PurchaseOrderItem> Items, decimal Total, decimal TotalExVat, string Currency, string OperatingChain, string SessionId, string? UserId, Guid CartId);

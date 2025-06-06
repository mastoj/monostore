namespace MonoStore.Contracts.Checkout;

[GenerateSerializer]
public record Product(string Id, string Name, decimal Price, decimal PriceExVat, string Url, string PrimaryImageUrl);
[GenerateSerializer]
public record PurchaseOrderItem(Product Product, int Quantity);

[GenerateSerializer]
public record PaymentInfo(string TransactionId, string PaymentMethod, string PaymentProvider, decimal Amount, string Currency, DateTimeOffset ProcessedAt, string Status);

[GenerateSerializer]
public record PurchaseOrderData(Guid Id, int Version, List<PurchaseOrderItem> Items, decimal Total, decimal TotalExVat, string Currency, string OperatingChain, string SessionId, string? UserId, Guid CartId, PaymentInfo? PaymentInfo = null);

public record Change(string ChangeType, DateTimeOffset TimeStamp, long Version, object data);
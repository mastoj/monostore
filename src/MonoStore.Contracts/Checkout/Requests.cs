namespace MonoStore.Contracts.Checkout.Requests;

[GenerateSerializer]
public record CreatePurchaseOrderRequest(Guid CartId);

[GenerateSerializer]
public record AddPaymentRequest(string TransactionId, string PaymentMethod, string PaymentProvider, decimal Amount, string Currency, DateTimeOffset ProcessedAt, string Status);
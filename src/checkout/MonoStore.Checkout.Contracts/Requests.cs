namespace MonoStore.Checkout.Contracts.Requests;

[GenerateSerializer]
public record CreatePurchaseOrderRequest(Guid PurchaseOrderId, Guid CartId);
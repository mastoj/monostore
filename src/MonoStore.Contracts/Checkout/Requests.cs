namespace MonoStore.Contracts.Checkout.Requests;

[GenerateSerializer]
public record CreatePurchaseOrderRequest(Guid CartId);
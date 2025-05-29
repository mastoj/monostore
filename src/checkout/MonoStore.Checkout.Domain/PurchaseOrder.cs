using MonoStore.Contracts.Checkout;

namespace MonoStore.Checkout.Domain;

#region Events
public record PurchaseOrderCreated(Guid PurchaseOrderId, PurchaseOrderItem[] Items, decimal Total, decimal totalExVat, string Currency, string OperatingChain, Guid CartId, string SessionId, string? UserId);
#endregion

public record PurchaseOrder(Guid Id, PurchaseOrderItem[] Items, decimal Total, decimal TotalExVat, string Currency, string OperatingChain, Guid CartId, string SessionId, string? UserId, int Version = 1)
{
  public static PurchaseOrder Create(PurchaseOrderCreated purchaseOrderCreated)
  {
    return new PurchaseOrder(
      purchaseOrderCreated.PurchaseOrderId,
      purchaseOrderCreated.Items,
      purchaseOrderCreated.Total,
      purchaseOrderCreated.totalExVat,
      purchaseOrderCreated.Currency,
      purchaseOrderCreated.OperatingChain,
      purchaseOrderCreated.CartId,
      purchaseOrderCreated.SessionId,
      purchaseOrderCreated.UserId
    );
  }
}

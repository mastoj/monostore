using MonoStore.Checkout.Contracts;

namespace MonoStore.Checkout.Module;

#region Events
public record PurchaseOrderCreated(Guid PurchaseOrderId, PurchaseOrderItem[] Items, decimal Total, decimal totalExVat, string Currency, string OperatingChain, Guid CartId, string SessionId, string? UserId);
#endregion

public record PurchaseOrder(Guid PurchaseOrderId, PurchaseOrderItem[] Items, decimal Total, decimal TotalExVat, string Currency, string OperatingChain, Guid CartId, string SessionId, string? UserId, int Version = 1)
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

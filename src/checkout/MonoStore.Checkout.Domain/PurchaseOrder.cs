using MonoStore.Contracts.Checkout;

namespace MonoStore.Checkout.Domain;

#region Events
public record PurchaseOrderCreated(Guid PurchaseOrderId, PurchaseOrderItem[] Items, decimal Total, decimal totalExVat, string Currency, string OperatingChain, Guid CartId, string SessionId, string? UserId);
public record PaymentAdded(Guid PurchaseOrderId, string TransactionId, string PaymentMethod, string PaymentProvider, decimal Amount, string Currency, DateTimeOffset ProcessedAt, string Status);
#endregion

public record PurchaseOrder(Guid Id, PurchaseOrderItem[] Items, decimal Total, decimal TotalExVat, string Currency, string OperatingChain, Guid CartId, string SessionId, string? UserId, PaymentInfo? PaymentInfo = null, int Version = 1)
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

  public PurchaseOrder AddPayment(PaymentAdded paymentAdded)
  {
    var paymentInfo = new PaymentInfo(
      paymentAdded.TransactionId,
      paymentAdded.PaymentMethod,
      paymentAdded.PaymentProvider,
      paymentAdded.Amount,
      paymentAdded.Currency,
      paymentAdded.ProcessedAt,
      paymentAdded.Status
    );

    return this with { PaymentInfo = paymentInfo, Version = Version + 1 };
  }
}

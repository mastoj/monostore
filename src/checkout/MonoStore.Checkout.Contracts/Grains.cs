using Monostore.Orleans.Types;
using MonoStore.Checkout.Contracts;

namespace Monostore.Checkout.Contracts.Grains;

public enum CheckoutErrorType
{
  Unkown = 99
}

[GenerateSerializer, Alias(nameof(CheckoutError))]
public record class CheckoutError
{
  [Id(0)]
  public string Message { get; set; } = "";
  [Id(1)]
  public CheckoutErrorType Type { get; set; } = CheckoutErrorType.Unkown;
}

[GenerateSerializer]
public record CreatePurchaseOrderMessage(Guid PurchaseOrderId, Guid CartId, string OperatingChain, string SessionId, string? UserId, PurchaseOrderItem[] Items);

[GenerateSerializer]
public record GetPurchaseOrder;

public interface IPurchaseOrderGrain : IGrainWithStringKey
{
  public static string PurchaseOrderGrainId(Guid purchaseOrderId) => $"purchaseorder/{purchaseOrderId.ToString().ToLower()}";
  Task<GrainResult<PurchaseOrderData, CheckoutError>> CreatePurchaseOrder(CreatePurchaseOrderMessage createPurchaseOrder);
  Task<GrainResult<PurchaseOrderData, CheckoutError>> GetPurchaseOrder(GetPurchaseOrder getPurchaseOrder);
}


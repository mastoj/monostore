using Monostore.Orleans.Types;

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
public record Product(string Id, string Name, decimal Price, decimal PriceExVat, string Url, string PrimaryImageUrl);
[GenerateSerializer]
public record PurchaseOrderItem(Product Product, int Quantity);
[GenerateSerializer]
public record PurchaseOrder(Guid PurchaseOrderId, PurchaseOrderItem[] Items, decimal Total, decimal VatAmount, string Currency, string OperatingChain, string SessionId, string? UserId);
[GenerateSerializer]
public record CreatePurchaseOrderMessage(Guid PurchaseOrderId, Guid CartId, string OperatingChain, string SessionId, string? UserId, PurchaseOrderItem[] Items);

public interface IPurchaseOrderGrain : IGrainWithStringKey
{
  public static string PurchaseOrderGrainId(Guid purchaseOrderId) => $"purchaseorder/{purchaseOrderId.ToString().ToLower()}";
  Task<GrainResult<PurchaseOrder, CheckoutError>> CreatePurchaseOrder(CreatePurchaseOrderMessage createPurchaseOrder);
}


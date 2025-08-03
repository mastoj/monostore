using Monostore.Orleans.Types;
using MonoStore.Contracts.Checkout;

namespace MonoStore.Contracts.Checkout.Grains;

public enum CheckoutErrorType
{
  Unkown = 99,
  PaymentAlreadyExists = 1,
  PaymentNotFound = 2,
  InvalidPaymentAmount = 3
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
public record AddPaymentMessage(string TransactionId, string PaymentMethod, string PaymentProvider, decimal Amount, string Currency, DateTimeOffset ProcessedAt, string Status);

[GenerateSerializer]
public record GetPurchaseOrder;


[GenerateSerializer, Alias(nameof(OrderPaidEvent))]
public class OrderPaidEvent
{
  [Id(0)]
  public Guid PurchaseOrderId { get; set; }
  [Id(1)]
  public string TransactionId { get; set; } = "";
  [Id(2)]
  public string PaymentMethod { get; set; } = "";
  [Id(3)]
  public string PaymentProvider { get; set; } = "";
  [Id(4)]
  public decimal Amount { get; set; }
  [Id(5)]
  public string Currency { get; set; } = "";
  [Id(6)]
  public DateTimeOffset ProcessedAt { get; set; }
  [Id(7)]
  public string Status { get; set; } = "";
}

public interface IPurchaseOrderGrain : IGrainWithStringKey
{
  public static string PurchaseOrderGrainId(Guid purchaseOrderId) => $"purchaseorder/{purchaseOrderId.ToString().ToLower()}";
  Task<GrainResult<PurchaseOrderData, CheckoutError>> CreatePurchaseOrder(CreatePurchaseOrderMessage createPurchaseOrder);
  Task<GrainResult<PurchaseOrderData, CheckoutError>> AddPayment(AddPaymentMessage addPayment);
  Task<GrainResult<PurchaseOrderData, CheckoutError>> GetPurchaseOrder(GetPurchaseOrder getPurchaseOrder);
}

public interface IPurchaseOrderReportingGrain : IGrainWithStringKey
{
  public static string ReportingGrainId => "purchase-order-reporting";
  Task StartListening();
  Task StopListening();
}
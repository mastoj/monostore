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


public record OrderItem(string ProductId, string ProductName, decimal Price, decimal PriceExVat, int Quantity);
public record PurchaseOrder(string OrderId, OrderItem[] Items, decimal Total, decimal VatAmount, string Currency, string OperatingChain, string SessionId, string? UserId);
public record CreatePurchaseOrder(string OperatingChain, string SessionId, string? UserId, OrderItem[] Items);

public interface ICartGrain : IGrainWithStringKey
{
  Task<GrainResult<PurchaseOrder, CheckoutError>> CreatePurchaseOrder(CreatePurchaseOrder createPurchaseOrder);
}

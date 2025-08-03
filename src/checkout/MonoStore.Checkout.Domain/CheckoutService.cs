using DotNext;
using MonoStore.Contracts.Checkout.Grains;

namespace MonoStore.Checkout.Domain;

public static class CheckoutService
{
  private static string GetCurrency(string operatingChain) => operatingChain switch
  {
    "OCNOELK" => "NOK",
    "OCSEELG" => "SEK",
    "OCDKELG" => "DKK",
    "OCFIGIG" => "Eur",
    _ => throw new InvalidOperationException("Unknown operating chain")
  };

  public static Result<PurchaseOrderCreated> Create(CreatePurchaseOrderMessage command)
  {
    // Todo: check that cart does not already exist
    var total = command.Items.Sum(i => i.Product.Price * i.Quantity);
    var totalExVat = command.Items.Sum(i => i.Product.PriceExVat * i.Quantity);
    var currency = GetCurrency(command.OperatingChain);
    return Result.FromValue(new PurchaseOrderCreated(command.PurchaseOrderId, command.Items, total, totalExVat, currency, command.OperatingChain, command.CartId, command.SessionId, command.UserId));
  }

  public static Result<PaymentAdded> Handle(PurchaseOrder purchaseOrder, AddPaymentMessage command)
  {
    if (purchaseOrder.PaymentInfo != null)
    {
      return Result.FromException<PaymentAdded>(new PaymentAlreadyAddedException(command.GetType().Name));
    }

    // Validate payment amount matches purchase order total
    if (command.Amount != purchaseOrder.Total)
    {
      return Result.FromException<PaymentAdded>(new PaymentAmountMismatchException(command.GetType().Name, purchaseOrder.Total, command.Amount));
    }

    var paymentEvent = new PaymentAdded(
      purchaseOrder.Id,
      command.TransactionId,
      command.PaymentMethod,
      command.PaymentProvider,
      command.Amount,
      command.Currency,
      command.ProcessedAt,
      command.Status
    );
    return Result.FromValue(paymentEvent);
  }
}

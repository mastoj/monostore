using DotNext;
using JasperFx.CodeGeneration.Model;
using Monostore.Checkout.Contracts.Grains;

namespace MonoStore.Checkout.Module;

internal static class CheckoutService
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

}

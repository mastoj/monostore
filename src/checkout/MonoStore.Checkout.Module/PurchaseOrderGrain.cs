using Monostore.Checkout.Contracts.Grains;
using Monostore.Orleans.Types;

namespace MonoStore.Checkout.Module;

public class PurchaseOrderGrain : Grain, IPurchaseOrderGrain
{
  public Task<GrainResult<PurchaseOrderData, CheckoutError>> CreatePurchaseOrder(CreatePurchaseOrderMessage createPurchaseOrder)
  {
    var orderData = new PurchaseOrderData(
      createPurchaseOrder.PurchaseOrderId,
      createPurchaseOrder.Items,
      createPurchaseOrder.Items.Sum(i => i.Product.Price * i.Quantity),
      createPurchaseOrder.Items.Sum(i => i.Product.PriceExVat * i.Quantity),
      "NOK",
      createPurchaseOrder.OperatingChain,
      createPurchaseOrder.SessionId,
      createPurchaseOrder.UserId
    );
    return Task.FromResult(GrainResult<PurchaseOrderData, CheckoutError>.Success(orderData));
  }
}

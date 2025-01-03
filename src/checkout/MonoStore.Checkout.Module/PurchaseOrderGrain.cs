using Monostore.Checkout.Contracts.Grains;
using Monostore.Orleans.Types;
using MonoStore.Checkout.Contracts;
using MonoStore.Marten;
using static MonoStore.Checkout.Module.CheckoutService;

namespace MonoStore.Checkout.Module;

internal static class Mappers
{
  internal static PurchaseOrderData AsContract(this PurchaseOrder purchaseOrder)
  {
    return new PurchaseOrderData(purchaseOrder.Id, purchaseOrder.Version, purchaseOrder.Items.ToList(), purchaseOrder.Total, purchaseOrder.TotalExVat, purchaseOrder.Currency, purchaseOrder.OperatingChain, purchaseOrder.SessionId, purchaseOrder.UserId, purchaseOrder.CartId);
  }
}


public class PurchaseOrderGrain : Grain, IPurchaseOrderGrain
{

  private IEventStore eventStore;

  private readonly ILogger<PurchaseOrderGrain> logger;
  private PurchaseOrder? _currentPurchaseOrder;
  private PurchaseOrder CurrentPurchaseOrder
  {
    get => _currentPurchaseOrder ?? throw new InvalidOperationException("Purchase order not found");
    set
    {
      _currentPurchaseOrder = value;
      logger.LogInformation("Purchase order {purchaseOrderId} updated to version {version}", value.Id, value.Version);
    }
  }

  public PurchaseOrderGrain(IEventStore eventStore, ILogger<PurchaseOrderGrain> logger)
  {
    this.eventStore = eventStore;
    this.logger = logger;
  }

  public override async Task OnActivateAsync(CancellationToken cancellationToken)
  {
    logger.LogInformation("Activating {grainKey}", this.GetPrimaryKeyString());
    var id = Guid.Parse(this.GetPrimaryKeyString().Split("/")[1]);
    _currentPurchaseOrder = await eventStore.GetState<PurchaseOrder>(id, default);
    await base.OnActivateAsync(cancellationToken);
  }

  public async Task<GrainResult<PurchaseOrderData, CheckoutError>> CreatePurchaseOrder(CreatePurchaseOrderMessage createPurchaseOrder)
  {
    var result = Create(createPurchaseOrder);
    if (result.IsSuccessful)
    {
      CurrentPurchaseOrder = await eventStore.CreateStream(createPurchaseOrder.PurchaseOrderId, result.Value, PurchaseOrder.Create, default);
    }
    return GrainResult<PurchaseOrderData, CheckoutError>.Success(CurrentPurchaseOrder.AsContract());
  }
}

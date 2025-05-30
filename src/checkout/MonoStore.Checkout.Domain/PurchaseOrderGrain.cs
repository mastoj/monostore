using MonoStore.Contracts.Checkout.Grains;
using Monostore.Orleans.Types;
using MonoStore.Contracts.Checkout;
using MonoStore.Marten;
using static MonoStore.Checkout.Domain.CheckoutService;
using Microsoft.Extensions.Logging;


namespace MonoStore.Checkout.Domain;

internal static class Mappers
{
  internal static PurchaseOrderData AsContract(this PurchaseOrder purchaseOrder)
  {
    return new PurchaseOrderData(
      purchaseOrder.Id,
      purchaseOrder.Version,
      purchaseOrder.Items.ToList(),
      purchaseOrder.Total,
      purchaseOrder.TotalExVat,
      purchaseOrder.Currency,
      purchaseOrder.OperatingChain,
      purchaseOrder.SessionId,
      purchaseOrder.UserId,
      purchaseOrder.CartId,
      purchaseOrder.PaymentInfo);
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

  public async Task<GrainResult<PurchaseOrderData, CheckoutError>> AddPayment(AddPaymentMessage addPayment)
  {
    // Check if payment already exists
    if (CurrentPurchaseOrder.PaymentInfo != null)
    {
      return GrainResult<PurchaseOrderData, CheckoutError>.Failure(new CheckoutError
      {
        Message = "Payment already exists for this purchase order",
        Type = CheckoutErrorType.PaymentAlreadyExists
      });
    }

    // Validate payment amount matches purchase order total
    if (addPayment.Amount != CurrentPurchaseOrder.Total)
    {
      return GrainResult<PurchaseOrderData, CheckoutError>.Failure(new CheckoutError
      {
        Message = $"Payment amount {addPayment.Amount} does not match purchase order total {CurrentPurchaseOrder.Total}",
        Type = CheckoutErrorType.InvalidPaymentAmount
      });
    }

    var paymentEvent = new PaymentAdded(
      CurrentPurchaseOrder.Id,
      addPayment.TransactionId,
      addPayment.PaymentMethod,
      addPayment.PaymentProvider,
      addPayment.Amount,
      addPayment.Currency,
      addPayment.ProcessedAt,
      addPayment.Status
    );

    CurrentPurchaseOrder = await eventStore.AppendToStream(CurrentPurchaseOrder.Id, paymentEvent, CurrentPurchaseOrder.Version, CurrentPurchaseOrder.AddPayment, default);
    return GrainResult<PurchaseOrderData, CheckoutError>.Success(CurrentPurchaseOrder.AsContract());
  }

  public Task<GrainResult<PurchaseOrderData, CheckoutError>> GetPurchaseOrder(GetPurchaseOrder getPurchaseOrder)
  {
    return Task.FromResult(GrainResult<PurchaseOrderData, CheckoutError>.Success(CurrentPurchaseOrder.AsContract()));
  }
}

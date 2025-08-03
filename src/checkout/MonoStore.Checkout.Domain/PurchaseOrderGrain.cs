using MonoStore.Contracts.Checkout.Grains;
using Monostore.Orleans.Types;
using MonoStore.Contracts.Checkout;
using MonoStore.Marten;
using static MonoStore.Checkout.Domain.CheckoutService;
using Microsoft.Extensions.Logging;
using Orleans.Streams;

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
  private IAsyncStream<OrderPaidEvent>? centralOrderPaidStream;

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
    var streamProvider = this.GetStreamProvider("OrderStreamProvider");
    centralOrderPaidStream = streamProvider.GetStream<OrderPaidEvent>(StreamId.Create("OrderPaidEvent", Guid.Empty));
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
    var result = Handle(CurrentPurchaseOrder, addPayment);
    if (!result.IsSuccessful)
    {
      return GrainResult<PurchaseOrderData, CheckoutError>.Failure(new CheckoutError
      {
        Message = result.Error.Message,
        Type = result.Error switch
        {
          PaymentAlreadyAddedException => CheckoutErrorType.PaymentAlreadyExists,
          PaymentAmountMismatchException => CheckoutErrorType.InvalidPaymentAmount,
          _ => CheckoutErrorType.Unkown
        }
      });
    }
    CurrentPurchaseOrder = await eventStore.AppendToStream(CurrentPurchaseOrder.Id, result.Value, CurrentPurchaseOrder.Version, CurrentPurchaseOrder.AddPayment, default);

    // Publish OrderPaidEvent to stream
    var paidEvent = new OrderPaidEvent
    {
      PurchaseOrderId = CurrentPurchaseOrder.Id,
      TransactionId = addPayment.TransactionId,
      PaymentMethod = addPayment.PaymentMethod,
      PaymentProvider = addPayment.PaymentProvider,
      Amount = addPayment.Amount,
      Currency = addPayment.Currency,
      ProcessedAt = addPayment.ProcessedAt,
      Status = addPayment.Status
    };

    // Publish to the central reporting stream
    if (centralOrderPaidStream != null)
    {
      await centralOrderPaidStream.OnNextAsync(paidEvent);
    }

    return GrainResult<PurchaseOrderData, CheckoutError>.Success(CurrentPurchaseOrder.AsContract());
  }

  public Task<GrainResult<PurchaseOrderData, CheckoutError>> GetPurchaseOrder(GetPurchaseOrder getPurchaseOrder)
  {
    return Task.FromResult(GrainResult<PurchaseOrderData, CheckoutError>.Success(CurrentPurchaseOrder.AsContract()));
  }
}

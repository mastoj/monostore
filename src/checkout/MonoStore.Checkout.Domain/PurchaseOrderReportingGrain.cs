using Microsoft.Extensions.Logging;
using MonoStore.Contracts.Checkout.Grains;
using Orleans.Streams;

namespace MonoStore.Checkout.Domain;

public class PurchaseOrderReportingGrain : Grain, IPurchaseOrderReportingGrain
{
  private readonly ILogger<PurchaseOrderReportingGrain> logger;
  private IAsyncStream<OrderPaidEvent>? orderPaidStream;
  private StreamSubscriptionHandle<OrderPaidEvent>? subscriptionHandle;

  public PurchaseOrderReportingGrain(ILogger<PurchaseOrderReportingGrain> logger)
  {
    this.logger = logger;
  }

  public override async Task OnActivateAsync(CancellationToken cancellationToken)
  {
    logger.LogInformation("Activating PurchaseOrderReportingGrain singleton");

    // Subscribe to all OrderPaidEvent streams using a wildcard pattern
    // This ensures we capture events from all purchase orders
    await StartListening();

    await base.OnActivateAsync(cancellationToken);
  }

  public async Task StartListening()
  {
    try
    {
      logger.LogInformation("Starting to listen for OrderPaidEvents");

      var streamProvider = this.GetStreamProvider("OrderStreamProvider");

      // Subscribe to a centralized OrderPaidEvent stream
      // All purchase orders will publish to this shared stream
      orderPaidStream = streamProvider.GetStream<OrderPaidEvent>(StreamId.Create("OrderPaidEvent", Guid.Empty));

      subscriptionHandle = await orderPaidStream.SubscribeAsync(
          OnNextAsync,
          OnErrorAsync,
          OnCompletedAsync);

      logger.LogInformation("Successfully started listening for OrderPaidEvents");
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Failed to start listening for OrderPaidEvents");
      throw;
    }
  }

  public async Task StopListening()
  {
    try
    {
      if (subscriptionHandle != null)
      {
        logger.LogInformation("Stopping OrderPaidEvent subscription");
        await subscriptionHandle.UnsubscribeAsync();
        subscriptionHandle = null;
      }
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Failed to stop listening for OrderPaidEvents");
      throw;
    }
  }

  public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
  {
    logger.LogInformation("Deactivating PurchaseOrderReportingGrain singleton, reason: {reason}", reason);
    await StopListening();
    await base.OnDeactivateAsync(reason, cancellationToken);
  }

  // Stream subscription observer methods
  public Task OnNextAsync(OrderPaidEvent item, StreamSequenceToken? token = null)
  {
    logger.LogInformation(
        "OrderPaidEvent received: PurchaseOrderId={PurchaseOrderId}, TransactionId={TransactionId}, Amount={Amount} {Currency}, PaymentMethod={PaymentMethod}, Status={Status}",
        item.PurchaseOrderId,
        item.TransactionId,
        item.Amount,
        item.Currency,
        item.PaymentMethod,
        item.Status);

    Console.WriteLine($"OrderPaidEvent received: {item.PurchaseOrderId} - {item.TransactionId} - {item.Amount} {item.Currency} - {item.PaymentMethod} - {item.Status}");

    // TODO: Add your reporting logic here
    // For example: update reporting database, send notifications, etc.

    return Task.CompletedTask;
  }

  public Task OnErrorAsync(Exception ex)
  {
    logger.LogError(ex, "Error in OrderPaidEvent stream subscription");

    // Attempt to restart subscription on error
    _ = Task.Run(async () =>
    {
      try
      {
        await Task.Delay(5000); // Wait 5 seconds before retry
        await StartListening();
        logger.LogInformation("Successfully restarted OrderPaidEvent subscription after error");
      }
      catch (Exception retryEx)
      {
        logger.LogError(retryEx, "Failed to restart OrderPaidEvent subscription after error");
      }
    });

    return Task.CompletedTask;
  }

  public Task OnCompletedAsync()
  {
    logger.LogInformation("OrderPaidEvent stream subscription completed");
    return Task.CompletedTask;
  }
}

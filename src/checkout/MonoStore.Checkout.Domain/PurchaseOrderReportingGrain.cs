using Microsoft.Extensions.Logging;
using MonoStore.Contracts.Checkout.Grains;
using MonoStore.Contracts.Checkout;
using Orleans.Streams;

namespace MonoStore.Checkout.Domain;

[KeepAlive]
[ImplicitStreamSubscription(StreamConstants.OrderPaidEventNamespace)]
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
      logger.LogInformation("Starting to listen for OrderPaidEvents using ImplicitStreamSubscription");

      var streamProvider = this.GetStreamProvider(StreamConstants.OrderStreamProviderName);

      // Use a fixed GUID for the reporting grain stream
      var streamId = StreamId.Create(StreamConstants.OrderPaidEventNamespace, StreamConstants.ReportingGrainStreamGuid);
      orderPaidStream = streamProvider.GetStream<OrderPaidEvent>(streamId);

      logger.LogInformation("Setting up stream subscription with StreamId: {StreamId}, GUID: {Guid}",
        streamId, StreamConstants.ReportingGrainStreamGuid);

      // With ImplicitStreamSubscription, we just need to call SubscribeAsync
      // The runtime automatically creates subscriptions for grains with the attribute
      subscriptionHandle = await orderPaidStream.SubscribeAsync(
          async (item, token) =>
          {
            logger.LogWarning("🚨🚨🚨 RECEIVED EVENT! Item: {PurchaseOrderId}", item.PurchaseOrderId);
            await OnNextAsync(item, token);
          });

      logger.LogInformation("Successfully set up implicit stream subscription. Handle ID: {HandleId}",
        subscriptionHandle?.HandleId);
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
    logger.LogWarning("🚨 OnNextAsync CALLED! This should be visible!");

    logger.LogInformation(
        "[PurchaseOrderReportingGrain] OrderPaidEvent received: PurchaseOrderId={PurchaseOrderId}, TransactionId={TransactionId}, Amount={Amount} {Currency}, PaymentMethod={PaymentMethod}, Status={Status}",
        item.PurchaseOrderId,
        item.TransactionId,
        item.Amount,
        item.Currency,
        item.PaymentMethod,
        item.Status);

    Console.WriteLine($"[PurchaseOrderReportingGrain] OrderPaidEvent received (console): {item.PurchaseOrderId} - {item.TransactionId} - {item.Amount} {item.Currency} - {item.PaymentMethod} - {item.Status}");

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

  public async Task TestPublishEvent()
  {
    try
    {
      logger.LogInformation("Testing: Publishing a test event to the same stream we're subscribed to");

      var streamProvider = this.GetStreamProvider(StreamConstants.OrderStreamProviderName);
      var streamId = StreamId.Create(StreamConstants.OrderPaidEventNamespace, StreamConstants.ReportingGrainStreamGuid);
      var testStream = streamProvider.GetStream<OrderPaidEvent>(streamId);

      logger.LogInformation("Publisher stream details - StreamId: {StreamId}, GUID: {StreamGuid}, StreamProvider: {StreamProvider}",
        streamId, StreamConstants.ReportingGrainStreamGuid, streamProvider.GetType().Name);

      var testEvent = new OrderPaidEvent
      {
        PurchaseOrderId = Guid.NewGuid(),
        TransactionId = "TEST-TRANSACTION",
        PaymentMethod = "TEST",
        PaymentProvider = "TEST",
        Amount = 100.00m,
        Currency = "USD",
        ProcessedAt = DateTimeOffset.UtcNow,
        Status = "TEST"
      };

      logger.LogInformation("Publishing test event: {PurchaseOrderId} - {TransactionId}",
        testEvent.PurchaseOrderId, testEvent.TransactionId);

      await testStream.OnNextAsync(testEvent);
      logger.LogInformation("Test event published successfully");


    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Failed to publish test event");
    }
  }


}

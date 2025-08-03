using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MonoStore.Contracts.Checkout.Grains;

namespace MonoStore.Checkout.Domain;

/// <summary>
/// Background service that ensures the PurchaseOrderReportingGrain singleton stays active
/// </summary>
public class PurchaseOrderReportingService : BackgroundService
{
  private readonly IServiceProvider serviceProvider;
  private readonly ILogger<PurchaseOrderReportingService> logger;

  public PurchaseOrderReportingService(
      IServiceProvider serviceProvider,
      ILogger<PurchaseOrderReportingService> logger)
  {
    this.serviceProvider = serviceProvider;
    this.logger = logger;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    logger.LogInformation("Starting PurchaseOrderReportingService");

    try
    {
      // Wait longer for Orleans to be fully initialized
      logger.LogInformation("Waiting for Orleans to initialize...");
      await Task.Delay(10000, stoppingToken); // Increased to 10 seconds

      while (!stoppingToken.IsCancellationRequested)
      {
        try
        {
          using var scope = serviceProvider.CreateScope();
          var grainFactory = scope.ServiceProvider.GetRequiredService<IGrainFactory>();

          logger.LogDebug("Getting PurchaseOrderReportingGrain with ID: {GrainId}", IPurchaseOrderReportingGrain.ReportingGrainId);

          // Get the singleton reporting grain and ensure it's listening
          var reportingGrain = grainFactory.GetGrain<IPurchaseOrderReportingGrain>(
              IPurchaseOrderReportingGrain.ReportingGrainId);

          logger.LogDebug("Successfully got grain reference, calling StartListening...");

          // First just try to ping the grain without starting listening
          // This will tell us if the grain can be activated at all
          await reportingGrain.StartListening();

          logger.LogInformation("Successfully called StartListening on PurchaseOrderReportingGrain");

          // Wait 30 seconds before next ping
          await Task.Delay(30000, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
          break;
        }
        catch (Exception ex)
        {
          logger.LogError(ex, "Error while pinging PurchaseOrderReportingGrain, will retry");
          await Task.Delay(10000, stoppingToken); // Wait 10 seconds on error
        }
      }
    }
    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
    {
      logger.LogInformation("PurchaseOrderReportingService was cancelled");
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Fatal error in PurchaseOrderReportingService");
    }

    logger.LogInformation("PurchaseOrderReportingService stopped");
  }
}

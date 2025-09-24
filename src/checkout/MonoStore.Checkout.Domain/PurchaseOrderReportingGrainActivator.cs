using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MonoStore.Contracts.Checkout.Grains;

namespace MonoStore.Checkout.Domain;

public class PurchaseOrderReportingGrainActivator : IStartupTask
{
  private readonly ILogger<PurchaseOrderReportingGrainActivator> _logger;
  private readonly IGrainFactory _grainFactory;

  public PurchaseOrderReportingGrainActivator(IGrainFactory grainFactory, ILogger<PurchaseOrderReportingGrainActivator> logger)
  {
    _grainFactory = grainFactory;
    _logger = logger;
  }

  public async Task Execute(CancellationToken cancellationToken)
  {
    var grain = _grainFactory.GetGrain<IPurchaseOrderReportingGrain>("singleton");
    await grain.StartListening();
  }
}

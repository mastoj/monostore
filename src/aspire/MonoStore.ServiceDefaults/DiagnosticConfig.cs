using System.Diagnostics.Metrics;

namespace Monostore.ServiceDefaults;
public static class DiagnosticConfig
{
  public static string serviceName = "monostore.api";
  public static Meter meter = new Meter(serviceName);
  public static Counter<long> CreateCartCounter = meter.CreateCounter<long>("cart.create");
  public static Histogram<long> CartValue = meter.CreateHistogram<long>("cart.value");

}
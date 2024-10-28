using System.Diagnostics.Metrics;

namespace Monostore.ServiceDefaults;
public static class DiagnosticConfig
{
  public static string apiServiceName = "monostore-api";
  public static string orleansDashboardServiceName = "monostore.orleans.dashboard";
  public static Meter GetMeter(string serviceName) => new Meter(serviceName);
  public static Counter<long> CreateCartCounter => GetMeter(apiServiceName).CreateCounter<long>("cart.create");
  public static Histogram<long> CartValue => GetMeter(apiServiceName).CreateHistogram<long>("cart.value");

}
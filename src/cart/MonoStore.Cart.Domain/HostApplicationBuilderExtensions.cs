using Marten.Events.Projections;
using Microsoft.Extensions.Hosting;

namespace MonoStore.Cart.Domain;

public static class HostApplicationBuilderExtensions
{
  public static IHostApplicationBuilder UseCartService(this IHostApplicationBuilder builder)
  {
    // Configure Marten event store with the provided options
    // builder.Services.AddMartenEventStore(connectionString, databaseName, configure);
    // return builder;
    return builder.UseMartenEventStore("monostorepg", "cart", so =>
    {
      so.Projections.Snapshot<Cart>(SnapshotLifecycle.Inline);
      return so;
    });
  }
}

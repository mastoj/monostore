using dotenv.net;
using Orleans.Configuration;

namespace Microsoft.Extensions.Hosting;

public static class Hosting
{
  private static TimeSpan DefaultCollectionInterval = TimeSpan.FromSeconds(10);
  private static TimeSpan DefaultCollectionAge = TimeSpan.FromSeconds(20);
  public static HostApplicationBuilder UseHosting(this HostApplicationBuilder builder, string defaultServiceName, TimeSpan? collectionInterval = null, TimeSpan? collectionAge = null)
  {
    DotEnv.Load();

    builder.AddServiceDefaults();
    builder.AddKeyedAzureTableServiceClient("clustering", settings => settings.DisableTracing = true);
    builder.AddKeyedAzureBlobServiceClient("grainstate", settings => settings.DisableTracing = true);
    builder.UseOrleans(siloBuilder =>
    {
      siloBuilder
              .AddActivityPropagation()
              .UseDashboard(x => x.HostSelf = true)
              .AddMemoryStreams("ProductStreamProvider")
              .AddMemoryStreams("OrderStreamProvider")
              .AddMemoryGrainStorage("PubSubStore")
              .Configure<GrainCollectionOptions>(options =>
                    {
                      options.CollectionAge = collectionAge ?? DefaultCollectionAge;
                      options.CollectionQuantum = collectionInterval ?? DefaultCollectionInterval;
                    });
    });

    return builder;
  }
}

﻿using dotenv.net;
using Monostore.ServiceDefaults;
using OpenTelemetry.Resources;
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
    builder.AddKeyedAzureTableClient("clustering", settings => settings.DisableTracing = true);
    builder.AddKeyedAzureBlobClient("grainstate", settings => settings.DisableTracing = true);
    builder.UseOrleans(siloBuilder =>
    {
      siloBuilder
              .AddActivityPropagation()
              .UseDashboard(x => x.HostSelf = true)
              .AddMemoryStreams("ProductStreamProvider")
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

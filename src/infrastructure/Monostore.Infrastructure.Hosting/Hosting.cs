using dotenv.net;
using Monostore.ServiceDefaults;
using OpenTelemetry.Resources;
using Orleans.Configuration;

namespace Microsoft.Extensions.Hosting;

public static class Hosting
{
  public static HostApplicationBuilder UseHosting(this HostApplicationBuilder builder, string defaultServiceName)
  {
    DotEnv.Load();

    var serviceName = builder.Configuration["OTEL_RESOURCE_NAME"] ?? defaultServiceName;

    var attributes = builder.Configuration["OTEL_RESOURCE_ATTRIBUTES"]?.Split(',').Select(s => s.Split("=")) ?? [];
    var serviceInstanceId = attributes.FirstOrDefault(y => y[0].Contains("service.instance.id"))?[1] ?? throw new Exception("Service instance id not found");

    builder.AddServiceDefaults(c =>
        {
          c.AddService(serviceName, serviceInstanceId: serviceInstanceId);
        }, cm =>
        {
          cm.AddMeter(DiagnosticConfig.GetMeter(serviceName).Name);
        });
    builder.AddKeyedAzureTableClient("clustering");
    builder.AddKeyedAzureBlobClient("grainstate");

    builder.UseOrleans(siloBuilder =>
    {
      siloBuilder
              .AddActivityPropagation()
              .UseDashboard(x => x.HostSelf = true)
              .AddMemoryStreams("ProductStreamProvider")
              .AddMemoryGrainStorage("PubSubStore")
              .Configure<GrainCollectionOptions>(options =>
                    {
                      options.CollectionAge = TimeSpan.FromSeconds(20);
                      options.CollectionQuantum = TimeSpan.FromSeconds(10);
                    });
    });

    return builder;
  }
}

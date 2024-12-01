using System.Net;
using dotenv.net;
using Monostore.ServiceDefaults;
using OpenTelemetry.Resources;
using Orleans.Configuration;

DotEnv.Load();

var builder = Host.CreateApplicationBuilder(args);
builder.AddCart();

var serviceName = builder.Configuration["OTEL_RESOURCE_NAME"] ?? "monostore-cart-host";

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

var host = builder.Build();
host.Run();

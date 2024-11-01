using System.Net;
using Monostore.ServiceDefaults;
using OpenTelemetry.Resources;
using Orleans.Configuration;

var builder = Host.CreateApplicationBuilder(args);
builder.AddCart();

var serviceName = builder.Configuration["OTEL_RESOURCE_NAME"] ?? "monostore-cart-host";
var serviceInstanceId = builder.Configuration["OTEL_RESOURCE_ATTRIBUTES"]?.Split('=') switch
{
[string k, string v] => v,
  _ => throw new Exception($"Invalid header format {builder.Configuration["OTEL_RESOURCE_ATTRIBUTES"]}")
};



builder.AddServiceDefaults(c =>
    {
      c.AddService(serviceName, serviceInstanceId: serviceInstanceId);
    }, cm =>
    {
      cm.AddMeter(DiagnosticConfig.GetMeter(serviceName).Name);
    });
builder.AddKeyedAzureTableClient("clustering");
builder.AddKeyedAzureBlobClient("grain-state");

builder.UseOrleans(static siloBuilder =>
{
  siloBuilder
          .UseLocalhostClustering(siloPort: 11112, gatewayPort: 30002, primarySiloEndpoint: new IPEndPoint(IPAddress.Loopback, 11111), serviceId: "monostore-orleans", clusterId: "monostore-orleans")
          .UseDashboard()
          .AddActivityPropagation()
          .Configure<GrainCollectionOptions>(options =>
                {
                  options.CollectionAge = TimeSpan.FromMinutes(10);
                  options.CollectionQuantum = TimeSpan.FromMinutes(5);
                });
});

var host = builder.Build();
host.Run();

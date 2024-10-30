using Monostore.ServiceDefaults;
using MonoStore.Cart.Host;
using OpenTelemetry.Resources;

var builder = Host.CreateApplicationBuilder(args);

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
          .UseDashboard()
          .AddActivityPropagation();
});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();

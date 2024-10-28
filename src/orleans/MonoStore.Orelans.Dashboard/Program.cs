using Microsoft.Extensions.Hosting;
using Monostore.ServiceDefaults;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);
var serviceName = builder.Configuration["OTEL_RESOURCE_NAME"] ?? "orleans-dashboard";
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
builder.Host.UseOrleans(siloBuilder =>
{
  // siloBuilder.UseAzureStorageClustering(o =>
  // {
  //   o.
  // })
  siloBuilder.UseDashboard(x => x.HostSelf = true);
});

var app = builder.Build();

app.Map("/dashboard", x => x.UseOrleansDashboard());

app.MapGet("/", () => "Hello World!");

app.Run();

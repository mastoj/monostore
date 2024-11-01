using System.Net;
using Monostore.ServiceDefaults;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);
var orleansServiceId = builder.Configuration["ORLEANS_SERVICE_ID"] ?? "monostore-orleans";
var orleansClusterId = builder.Configuration["ORLEANS_CLUSTER_ID"] ?? "monostore-orleans";
var orleansSiloPort = int.Parse(builder.Configuration["ORLEANS_SILO_PORT"]!);
var orleansGatewayPort = int.Parse(builder.Configuration["ORLEANS_GATEWAY_PORT"]!);
var orleansPrimarySiloPort = int.Parse(builder.Configuration["ORLEANS_PRIMARY_SILO_PORT"]!);
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
  siloBuilder.UseDashboard();
  siloBuilder.UseLocalhostClustering(siloPort: orleansSiloPort, gatewayPort: orleansGatewayPort, primarySiloEndpoint: new IPEndPoint(IPAddress.Loopback, orleansPrimarySiloPort), serviceId: orleansServiceId, clusterId: orleansClusterId);
  // siloBuilder.UseAzureStorageClustering(o =>
  // {
  //   o.
  // })
  // siloBuilder.UseDashboard(x => x.HostSelf = true);
});

var app = builder.Build();

app.Map("/dashboard", x => x.UseOrleansDashboard());

app.MapGet("/", () => "Hello World!");

app.Run();

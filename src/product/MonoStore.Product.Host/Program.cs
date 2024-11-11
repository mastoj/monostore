using System.Net;
using dotenv.net;
using Microsoft.Azure.Cosmos;
using Monostore.ServiceDefaults;
using MonoStore.Product.Module;
using OpenTelemetry.Resources;
using Orleans.Configuration;

DotEnv.Load();
Console.WriteLine("==> Product Host: Starting up!");
var connectionString = Environment.GetEnvironmentVariable("COSMOS_CONNECTION_STRING");
var builder = Host.CreateApplicationBuilder(args);

var serviceName = builder.Configuration["OTEL_RESOURCE_NAME"] ?? "monostore-product-host";
var attributes = builder.Configuration["OTEL_RESOURCE_ATTRIBUTES"]?.Split(',').Select(s => s.Split("=")) ?? [];
var serviceInstanceId = attributes.FirstOrDefault(y => y[0].Contains("service.instance.id"))?[1] ?? throw new Exception("Service instance id not found");


builder.AddServiceDefaults(c =>
    {
      c.AddService(serviceName, serviceInstanceId: serviceInstanceId);
    }, cm =>
    {
      cm.AddMeter(DiagnosticConfig.GetMeter(serviceName).Name);
    });
builder.Services.AddSingleton(new CosmosClient(connectionString));
builder.Services.AddSingleton<ProductRepository>();
builder.AddKeyedAzureTableClient("clustering");
builder.AddKeyedAzureBlobClient("grain-state");

builder.UseOrleans(siloBuilder =>
{
  siloBuilder
          .AddActivityPropagation()
          .UseDashboard(x => x.HostSelf = true)
          .Configure<GrainCollectionOptions>(options =>
                {
                  options.CollectionAge = TimeSpan.FromSeconds(10);
                  options.CollectionQuantum = TimeSpan.FromSeconds(5);
                });
});

var host = builder.Build();
host.Run();

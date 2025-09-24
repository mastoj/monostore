using MonoStore.Cart.Domain;
using MonoStore.Checkout.Domain;
using MonoStore.Product.Domain;
using JasperFx;
using dotenv.net;
using Orleans.Configuration;

var DefaultCollectionInterval = TimeSpan.FromSeconds(10);
var DefaultCollectionAge = TimeSpan.FromSeconds(20);

DotEnv.Load();
var builder = Host.CreateApplicationBuilder(args);

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
          .AddStartupTask<PurchaseOrderReportingGrainActivator>()
          .Configure<GrainCollectionOptions>(options =>
                {
                  options.CollectionAge = DefaultCollectionAge;
                  options.CollectionQuantum = DefaultCollectionInterval;
                });
});
builder.UseMartenEventStore("monostorepg", "monostore", so =>
{
  return so.AddCartProjections()
    .AddCheckoutProjections();
});

builder.AddProductService();

// builder.UseMartenEventStore("monostorepg", "cart", so =>
// {
//   so.Projections.Snapshot<Cart>(SnapshotLifecycle.Inline);
//   return so;
// });

var host = builder.Build();

await host.RunJasperFxCommands(args);

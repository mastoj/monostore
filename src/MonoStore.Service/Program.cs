using MonoStore.Cart.Domain;
using MonoStore.Checkout.Domain;
using MonoStore.Product.Domain;
using JasperFx;

var builder = Host.CreateApplicationBuilder(args).UseHosting("monostore-service");

builder.UseMartenEventStore("monostorepg", "monostore", so =>
{
  return so.AddCartProjections()
    .AddCheckoutProjections();
});

builder.AddProductService();

// Register the background service to keep the reporting grain active
builder.Services.AddHostedService<PurchaseOrderReportingService>();

// builder.UseMartenEventStore("monostorepg", "cart", so =>
// {
//   so.Projections.Snapshot<Cart>(SnapshotLifecycle.Inline);
//   return so;
// });

var host = builder.Build();

await host.RunJasperFxCommands(args);

using MonoStore.Cart.Domain;
using MonoStore.Checkout.Domain;

var builder = Host.CreateApplicationBuilder(args).UseHosting("monostore-service");

builder.UseMartenEventStore("monostorepg", "monostore", so =>
{
  return so.AddCartProjections()
    .AddCheckoutProjections();
});


// builder.UseMartenEventStore("monostorepg", "cart", so =>
// {
//   so.Projections.Snapshot<Cart>(SnapshotLifecycle.Inline);
//   return so;
// });

var host = builder.Build();
host.Run();

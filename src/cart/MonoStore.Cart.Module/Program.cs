using Marten.Events.Projections;
using MonoStore.Cart.Domain;

var builder = Host.CreateApplicationBuilder(args).UseHosting("monostore-cart-module");
builder.UseMartenEventStore("monostorepg", "cart", so =>
{
  so.Projections.Snapshot<Cart>(SnapshotLifecycle.Inline);
  return so;
});

var host = builder.Build();
host.Run();

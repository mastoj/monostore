using Marten.Events.Projections;
using MonoStore.Checkout.Domain;

var builder = Host.CreateApplicationBuilder(args).UseHosting("monostore-checkout-module");
builder.UseMartenEventStore("monostorepg", "checkout", so =>
{
  so.Projections.Snapshot<PurchaseOrder>(SnapshotLifecycle.Inline);
  return so;
});

var host = builder.Build();
host.Run();

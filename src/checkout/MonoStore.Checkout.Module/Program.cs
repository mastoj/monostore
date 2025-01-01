var builder = Host.CreateApplicationBuilder(args).UseHosting("monostore-checkout-module");
builder.UseMartenEventStore("monostorepg", "checkout");

var host = builder.Build();
host.Run();

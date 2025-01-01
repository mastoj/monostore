var builder = Host.CreateApplicationBuilder(args).UseHosting("monostore-cart-module");
builder.UseMartenEventStore("monostorepg", "cart");

var host = builder.Build();
host.Run();

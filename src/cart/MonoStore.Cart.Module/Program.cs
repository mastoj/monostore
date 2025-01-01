var builder = Host.CreateApplicationBuilder(args).UseHosting("monostore-cart-module");
builder.AddCart();

var host = builder.Build();
host.Run();

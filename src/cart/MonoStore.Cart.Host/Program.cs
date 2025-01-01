var builder = Host.CreateApplicationBuilder(args).UseHosting("monostore-cart-host");
builder.AddCart();

var host = builder.Build();
host.Run();

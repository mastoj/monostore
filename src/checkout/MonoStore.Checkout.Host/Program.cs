var builder = Host.CreateApplicationBuilder(args).UseHosting("monostore-checkout-host");

var host = builder.Build();
host.Run();

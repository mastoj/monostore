var builder = Host.CreateApplicationBuilder(args).UseHosting("monostore-checkout-module");

var host = builder.Build();
host.Run();

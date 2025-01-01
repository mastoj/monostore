using Microsoft.Azure.Cosmos;
using MonoStore.Product.Module;

var builder = Host.CreateApplicationBuilder(args).UseHosting("monostore-product-module");
var connectionString = Environment.GetEnvironmentVariable("COSMOS_CONNECTION_STRING");
builder.Services.AddSingleton(new CosmosClient(connectionString));
builder.Services.AddSingleton<ProductRepository>();

var host = builder.Build();
host.Run();

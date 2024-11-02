using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
  ["AppHost:BrowserToken"] = "",
});

var username = builder.AddParameter("postgres-username");
var password = builder.AddParameter("postgres-password");
var port = 5433;
var postgres = builder
  .AddPostgres("cart", username, password, port)
  // .WithDataVolume()
  .WithPgAdmin(a =>
    {
      a.WithHostPort(8888);
    });

var storage = builder.AddAzureStorage("storage").RunAsEmulator();
var clusteringTable = storage.AddTables("clustering");
var grainStorage = storage.AddBlobs("grain-state");

var orleans = builder.AddOrleans("default")
  .WithClustering(clusteringTable)
  .WithGrainStorage("default", grainStorage);

builder.AddProject<Projects.MonoStore_Api>("monostore-api")
  .WithReference(postgres)
  .WithReference(orleans.AsClient())
  .WithExternalHttpEndpoints();

builder.AddProject<Projects.MonoStore_Cart_Host>("monostore-cart-host")
  .WithReference(postgres)
  .WithReference(orleans);

builder.AddProject<Projects.MonoStore_Product_Host>("monostore-product-host")
  .WithReference(postgres)
  .WithReference(orleans)
  .WithReplicas(3);

builder.Build().Run();

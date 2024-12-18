using dotenv.net;
using Microsoft.Extensions.Configuration;

DotEnv.Load();

var builder = DistributedApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

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

var storage = builder.AddAzureStorage("storage").RunAsEmulator(x => x.WithImageTag("latest"));
var clusteringTable = storage.AddTables("clustering");
var grainStorage = storage.AddBlobs("grainstate");

var orleans = builder.AddOrleans("default")
  .WithClustering(clusteringTable)
  .WithGrainStorage("default", grainStorage);

builder.AddProject<Projects.MonoStore_Api>("monostore-api")
  .WithReference(orleans.AsClient())
  .WithExternalHttpEndpoints();

builder.AddProject<Projects.MonoStore_Cart_Host>("monostore-cart-host")
  .WithReference(postgres)
  .WithReference(orleans)
  .WaitFor(postgres)
  .WithReplicas(3);

builder.AddProject<Projects.MonoStore_Product_Host>("monostore-product-host")
  .WithReference(orleans)
  .WithReplicas(5);

builder.AddProject<Projects.MonoStore_Orelans_Dashboard>("orleans-dashboard")
  .WithReference(orleans)
  .WithExternalHttpEndpoints();

// builder.AddProject<Projects.Dummy>("dummy");

builder.Build().Run();

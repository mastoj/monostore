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
var orleansServiceId = "monostore-orleans";
var orleansClusterId = "monostore-orleans";
var productSiloPort = 11111;
var productGatewayPort = 30001;
var cartSiloPort = 11112;
var cartGatewayPort = 30002;
var dashboardSiloPort = 11113;
var dashboardGatewayPort = 30000;
//var gatewayPorts = $"{cartGatewayPort}";
var gatewayPorts = $"{productGatewayPort},{cartGatewayPort}";
//var gatewayPorts = $"{productGatewayPort}";

var orleans = builder.AddOrleans("default")
  .WithClustering(clusteringTable)
  .WithGrainStorage("default", grainStorage);

builder.AddProject<Projects.MonoStore_Orelans_Dashboard>("orleans-dashboard")
  .WithReference(orleans.AsClient())
  .WithEnvironment("ORLEANS_SERVICE_ID", orleansServiceId)
  .WithEnvironment("ORLEANS_CLUSTER_ID", orleansClusterId)
  .WithEnvironment("ORLEANS_SILO_PORT", dashboardSiloPort.ToString())
  .WithEnvironment("ORLEANS_GATEWAY_PORT", dashboardGatewayPort.ToString())
  .WithEnvironment("ORLEANS_PRIMARY_SILO_PORT", dashboardSiloPort.ToString())
  .WithExternalHttpEndpoints();

builder.AddProject<Projects.MonoStore_Api>("monostore-api")
  .WithReference(postgres)
  .WithReference(orleans.AsClient())
  .WithEnvironment("ORLEANS_SERVICE_ID", orleansServiceId)
  .WithEnvironment("ORLEANS_CLUSTER_ID", orleansClusterId)
  .WithEnvironment("ORLEANS_GATEWAY_PORTS", gatewayPorts)
  .WithExternalHttpEndpoints();

builder.AddProject<Projects.MonoStore_Cart_Host>("monostore-cart-host")
  .WithReference(postgres)
  .WithEnvironment("ORLEANS_SERVICE_ID", orleansServiceId)
  .WithEnvironment("ORLEANS_CLUSTER_ID", orleansClusterId)
  .WithEnvironment("ORLEANS_SILO_PORT", cartSiloPort.ToString())
  .WithEnvironment("ORLEANS_GATEWAY_PORT", cartGatewayPort.ToString())
  .WithEnvironment("ORLEANS_PRIMARY_SILO_PORT", dashboardSiloPort.ToString())
  .WithReference(orleans);

builder.AddProject<Projects.MonoStore_Product_Host>("monostore-product-host")
  .WithReference(postgres)
  .WithEnvironment("ORLEANS_SERVICE_ID", orleansServiceId)
  .WithEnvironment("ORLEANS_CLUSTER_ID", orleansClusterId)
  .WithEnvironment("ORLEANS_SILO_PORT", productSiloPort.ToString())
  .WithEnvironment("ORLEANS_GATEWAY_PORT", productGatewayPort.ToString())
  .WithEnvironment("ORLEANS_PRIMARY_SILO_PORT", dashboardSiloPort.ToString())
  .WithReference(orleans);
// .WithReplicas(3);

builder.Build().Run();

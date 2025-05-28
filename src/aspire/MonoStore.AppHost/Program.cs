using System.Diagnostics;
using dotenv.net;
using Microsoft.Extensions.Configuration;

EnsureDeveloperControlPaneIsNotRunning();

DotEnv.Load();

var builder = DistributedApplication.CreateBuilder(args);

var compose = builder.AddDockerComposeEnvironment("docker-env");
var containerApps = builder.AddAzureContainerAppEnvironment("containerapp-env");

builder.Configuration.AddEnvironmentVariables();

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
  ["AppHost:BrowserToken"] = "",
});

#pragma warning disable ASPIRECOMPUTE001 

var username = builder.AddParameter("postgres-username");
var password = builder.AddParameter("postgres-password");
var port = 5433;
var postgres = builder
  .AddPostgres("monostorepg", username, password, port)
  // .WithDataVolume()
  .WithPgAdmin(a =>
    {
      a.WithHostPort(8888);
    })
    //.WithComputeEnvironment(containerApps)
    .WithComputeEnvironment(compose);

var storage = builder.AddAzureStorage("storage").RunAsEmulator(x => x.WithImageTag("latest"));
var clusteringTable = storage.AddTables("clustering");
var grainStorage = storage.AddBlobs("grainstate");

var orleans = builder.AddOrleans("default")
  .WithClustering(clusteringTable)
  .WithGrainStorage("default", grainStorage);

var api = builder.AddProject<Projects.MonoStore_Api>("monostore-api")
  .WithReference(orleans.AsClient())
  .WithReference(postgres)
  .WaitFor(postgres)
  .WithExternalHttpEndpoints()
  //.WithComputeEnvironment(containerApps)
  .WithComputeEnvironment(compose);


builder.AddProject<Projects.MonoStore_Cart_Module>("monostore-cart-module")
  .WithReference(postgres)
  .WaitFor(postgres)
    .WithReference(orleans)
    .WithReplicas(3)
    .WithComputeEnvironment(containerApps)
    .WithComputeEnvironment(compose);


builder.AddProject<Projects.MonoStore_Checkout_Module>("monostore-checkout-module")
  .WithReference(postgres)
  .WaitFor(postgres)
  .WithReference(orleans)
  .WithReplicas(2)
//  .WithComputeEnvironment(containerApps)
  .WithComputeEnvironment(compose);


builder.AddProject<Projects.MonoStore_Product_Module>("monostore-product-module")
  .WithReference(orleans)
  .WithReplicas(3)
  .WithEnvironment("COSMOS_CONNECTION_STRING", Environment.GetEnvironmentVariable("COSMOS_CONNECTION_STRING"))
  .WithComputeEnvironment(compose)
  .WithComputeEnvironment(containerApps);


builder.AddProject<Projects.MonoStore_Orelans_Dashboard>("orleans-dashboard")
  .WithReference(orleans)
  .WithExternalHttpEndpoints()
  //.WithComputeEnvironment(containerApps)
  .WithComputeEnvironment(compose);

// if (builder.ExecutionContext.IsPublishMode)
// {
//   // Add a Dockerfile app, named "frontend", at "../frontend"
//   builder.AddDockerfile("monostore-backoffice", "../../backoffice")
//       // allow Aspire to control the port via env variable PORT and target port 3000
//       .WithHttpEndpoint(env: "PORT", targetPort: 3000)
//       // give the app an extenral endpoint
//       .WithExternalHttpEndpoints()
//    .WithComputeEnvironment(containerApps)
//       .WithComputeEnvironment(compose);

// }
// else
// {
//   builder.AddNpmApp("monostore-backoffice", "../../backoffice", "dev")
//     .WithHttpEndpoint(env: "PORT")
//     .WithEnvironment("NEXT_OTEL_VERBOSE", "1")
//     .WithEnvironment("OTEL_LOG_LEVEL", "debug")
//     .WithEnvironment("NODE_TLS_REJECT_UNAUTHORIZED", "0")
//     .WithReference(api)
//     .WaitFor(api)
//     .WithExternalHttpEndpoints();
// }

#pragma warning restore ASPIRECOMPUTE001
// builder.AddProject<Projects.Dummy>("dummy");

builder.Build().Run();


void EnsureDeveloperControlPaneIsNotRunning()
{
  const string processName = "dcpctrl"; // The Aspire Developer Control Pane process name
  var process = Process.GetProcesses()
      .SingleOrDefault(p => p.ProcessName.Contains(processName, StringComparison.OrdinalIgnoreCase));

  if (process == null) return;

  Console.WriteLine($"Shutting down developer control pane from previous run. Process: {process.ProcessName} (ID: {process.Id})");

  Thread.Sleep(TimeSpan.FromSeconds(5)); // Allow Docker containers to shut down to avoid orphaned containers

  try
  {
    process.Kill();
    Console.WriteLine($"Process {process.Id} killed successfully.");
  }
  catch (Exception ex)
  {
    Console.WriteLine($"Failed to kill process {process.Id}: {ex.Message}");
  }
}
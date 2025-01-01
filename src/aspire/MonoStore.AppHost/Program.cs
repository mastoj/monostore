using System.Diagnostics;
using dotenv.net;
using Microsoft.Extensions.Configuration;

EnsureDeveloperControlPaneIsNotRunning();

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
  .AddPostgres("monostorepg", username, password, port)
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

builder.AddProject<Projects.MonoStore_Cart_Module>("monostore-cart-module")
  .WithReference(postgres)
  .WithReference(orleans)
  .WaitFor(postgres)
  .WithReplicas(1);

builder.AddProject<Projects.MonoStore_Product_Module>("monostore-product-module")
  .WithReference(orleans)
  .WithReplicas(1);

builder.AddProject<Projects.MonoStore_Checkout_Module>("monostore-checkout-module")
  .WithReference(orleans)
  .WithReplicas(1);

builder.AddProject<Projects.MonoStore_Orelans_Dashboard>("orleans-dashboard")
  .WithReference(orleans)
  .WithExternalHttpEndpoints();

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
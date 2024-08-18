using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
  ["AppHost:BrowserToken"] = "",
});

var postgres = builder.AddPostgres("cart");
builder.AddProject<Projects.MonoStore_Api>("api")
  .WithReference(postgres)
  .WithExternalHttpEndpoints();

builder.Build().Run();

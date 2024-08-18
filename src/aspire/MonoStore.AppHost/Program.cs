using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
  ["AppHost:BrowserToken"] = "",
});

builder.AddProject<Projects.MonoStore_Api>("api").WithExternalHttpEndpoints();

builder.Build().Run();

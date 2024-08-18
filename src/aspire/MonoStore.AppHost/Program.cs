using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
  ["AppHost:BrowserToken"] = "",
});

var username = builder.AddParameter("postgres-username");
var password = builder.AddParameter("postgres-password");
var port = 5433;
Console.WriteLine($"Postgres: {username.Resource.Value}:{password.Resource.Value}@localhost:{port}");
var postgres = builder
  .AddPostgres("cart", username, password, port)
  .WithDataVolume()
  .WithPgAdmin(a =>
    {
      a.WithHostPort(8888);
    });
builder.AddProject<Projects.MonoStore_Api>("api")
  .WithReference(postgres)
  .WithExternalHttpEndpoints();

builder.Build().Run();

using System.Net;
using Monostore.ServiceDefaults;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddKeyedAzureTableClient("clustering");
builder.AddKeyedAzureBlobClient("grainstate");
builder.Host.UseOrleans(siloBuilder =>
{
  siloBuilder.UseDashboard(x => x.HostSelf = true);
});

var app = builder.Build();

app.Map("/dashboard", x => x.UseOrleansDashboard());

// Redirect to the dashboard
app.MapGet("/", async context =>
{
  context.Response.Redirect("/dashboard", true);
  await Task.CompletedTask;
});

app.Run();

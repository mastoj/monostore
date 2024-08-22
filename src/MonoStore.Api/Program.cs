using dotenv.net;
using Monostore.ServiceDefaults;
using MonoStore.Cart.Module;
using MonoStore.Product.Module;
using OpenTelemetry.Resources;
[assembly: GenerateCodeForDeclaringAssembly(typeof(MonoStore.Cart.Contracts.Cart))]

DotEnv.Load();

var config = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddConfiguration(config);
var serviceName = "monostore-api";

builder.AddServiceDefaults(c =>
{
    c.AddService(serviceName);
}, cm =>
{
    cm.AddMeter(DiagnosticConfig.meter.Name);
});

// Add services to the container.
// builder.Services.AddRazorPages();
builder.Host.UseOrleans(static siloBuilder =>
{
    siloBuilder
        .UseLocalhostClustering()
        .AddMemoryGrainStorage("carts")
        .AddActivityPropagation();
});

builder.AddNpgsqlDataSource("cart");

#region Domains
builder.AddCart();
builder.AddProduct();
#endregion

var app = builder.Build();

#region Endpoints
app.UseCart("cart");
#endregion

app.MapDefaultEndpoints();

// app.MapGroup("cart").MapCartEndpoints();

// // Configure the HTTP request pipeline.
// // if (!app.Environment.IsDevelopment())
// // {
// //     app.UseExceptionHandler("/Error");
// //     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
// //     app.UseHsts();
// // }

// // app.UseHttpsRedirection();
// // // app.UseStaticFiles();

// // app.UseRouting();

//app.UseAuthorization();

// // // app.MapRazorPages();

app.Run();

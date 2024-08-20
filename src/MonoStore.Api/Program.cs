using MonoStore.Cart.Module;
using OpenTelemetry.Resources;
[assembly: GenerateCodeForDeclaringAssembly(typeof(MonoStore.Cart.Contracts.Cart))]

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults(c =>
{
    c.AddService("monostore-api");
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

using MonoStore.Cart.Module;
[assembly: GenerateCodeForDeclaringAssembly(typeof(MonoStore.Cart.Contracts.Cart))]

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// Add services to the container.
// builder.Services.AddRazorPages();
builder.Host.UseOrleans(static siloBuilder =>
{
    siloBuilder
        .UseLocalhostClustering()
        .AddMemoryGrainStorage("carts");
});

builder.AddCart();
var app = builder.Build();

app.UseCart("cart");
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

// // app.UseAuthorization();

// // // app.MapRazorPages();

app.Run();

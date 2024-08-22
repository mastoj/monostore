using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using MonoStore.Product.Contracts;

namespace MonoStore.Product.Module;

public static class WebAppBuilderExtensions
{
  public static WebApplicationBuilder AddProduct(this WebApplicationBuilder app)
  {
    app.Services.AddTransient((sp) => new CosmosClient(app.Configuration["PRODUCT_COSMOSDB_CONNECTIONSTRING"]))
      .AddTransient<IProductService, ProductService>();
    return app;
  }
}

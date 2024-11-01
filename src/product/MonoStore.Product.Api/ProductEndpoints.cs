using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using MonoStore.Product.Contracts;
using MonoStore.Product.Contracts.Grains;

namespace MonoStore.Product.Api;

public static class ProductEndpoints
{
  public static void MapProductEndpoints(this IEndpointRouteBuilder routes)
  {
    routes.MapGet("/{id}", async (IGrainFactory grains, string id) =>
    {
      var productGrain = grains.GetGrain<IProductGrain>(id);
      return await productGrain.GetProductAsync(id, "OCNOELK");
    });
    routes.MapPost("/", async (IGrainFactory grains, ProductDto product) =>
    {
      var productGrain = grains.GetGrain<IProductGrain>(product.Sku);
      return await productGrain.UpdateProductAsync(product);
    });
  }

  public static WebApplication UseProduct(this WebApplication app, string groupPath)
  {
    app.MapGroup(groupPath).MapProductEndpoints();
    return app;
  }
}

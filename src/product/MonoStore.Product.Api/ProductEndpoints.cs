using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using MonoStore.Product.Contracts;
using MonoStore.Product.Contracts.Grains;

namespace MonoStore.Product.Api;

public static class ProductEndpoints
{
  private static string ProductGrainId(string operatingChain, string id) => $"product/{operatingChain.ToLower()}_{id.ToLower()}";

  public static void MapProductEndpoints(this IEndpointRouteBuilder routes)
  {
    routes.MapGet("/{operatingChain}/{id}", async (IGrainFactory grains, string operatingChain, string id) =>
    {
      var productGrainId = ProductGrainId(operatingChain, id);
      Console.WriteLine($"==> ProductGrainId: {productGrainId}");
      var productGrain = grains.GetGrain<IProductGrain>(productGrainId);
      return await productGrain.GetProductAsync();
    });
    routes.MapPost("/", async (IGrainFactory grains, ProductDetail product) =>
    {
      throw new NotImplementedException();
      // var productGrain = grains.GetGrain<IProductGrain>(product.Sku);
      // return await productGrain.UpdateProductAsync(product);
    });
  }

  public static WebApplication UseProduct(this WebApplication app, string groupPath)
  {
    Console.WriteLine("Adding UseProduct");
    app.MapGroup(groupPath).MapProductEndpoints();
    return app;
  }
}

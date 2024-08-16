namespace MonoStore.Cart.Module;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Orleans;

public static class TodoEndpoints
{
  public static void MapCartEndpoints(this IEndpointRouteBuilder routes)
  {
    routes.MapGet("/", async (IGrainFactory grains) =>
    {
      var cartGrain = grains.GetGrain<ICartGrain>("cart");
      return await cartGrain.GetCart("1");
    });

    routes.MapGet("/{id}", async (IGrainFactory grains, int id) =>
    {
      var cartGrain = grains.GetGrain<ICartGrain>("cart");
      return await cartGrain.GetCart(id.ToString());
    });

    routes.MapPost("/", async (IGrainFactory grains, AddItem addItem) =>
    {
      var cartGrain = grains.GetGrain<ICartGrain>("cart");
      var cart = await cartGrain.GetCart(addItem.CartId);
      return cart;
    });
  }
  // dotnet add package Microsoft.AspNetCore.App
}

namespace MonoStore.Cart.Module;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Orleans;

public static class TodoEndpoints
{
  public static string CartGrainId(string cartId) => $"cart/{cartId.ToLower()}";
  public static void MapCartEndpoints(this IEndpointRouteBuilder routes)
  {
    routes.MapPost("/", async (IGrainFactory grains, CreateCart createCart) =>
    {
      Console.WriteLine("CreateCart");
      var cartGrain = grains.GetGrain<ICartGrain>(CartGrainId(createCart.CartId.ToString()));
      return await cartGrain.CreateCart(createCart.CartId);
    });

    routes.MapPost("/{id}/items", async (IGrainFactory grains, Contracts.AddItem addItem) =>
    {
      var cartGrain = grains.GetGrain<ICartGrain>(CartGrainId(addItem.CartId));
      return await cartGrain.AddItem(addItem);
    });

    routes.MapDelete("/{id}/items/{productId}", async (IGrainFactory grains, Guid id, string productId) =>
    {
      var cartGrain = grains.GetGrain<ICartGrain>(CartGrainId(id.ToString()));
      return await cartGrain.RemoveItem(new Contracts.RemoveItem
      {
        CartId = id.ToString(),
        ProductId = productId
      });
    });

    routes.MapGet("/{id}", async (IGrainFactory grains, string id) =>
    {
      var cartGrain = grains.GetGrain<ICartGrain>(CartGrainId(id));
      return await cartGrain.GetCart(Guid.Parse(id));
    });

    // routes.MapGet("/", async (IGrainFactory grains) =>
    // {
    //   var cartGrain = grains.GetGrain<ICartGrain>("cart");
    //   return await cartGrain.GetCart("1");
    // });

    // routes.MapGet("/{id}", async (IGrainFactory grains, int id) =>
    // {
    //   var cartGrain = grains.GetGrain<ICartGrain>("cart" + id);
    //   return await cartGrain.GetCart(id.ToString());
    // });

    // routes.MapPost("/", async (IGrainFactory grains, AddItem addItem) =>
    // {
    //   var cartGrain = grains.GetGrain<ICartGrain>("cart");
    //   var cart = await cartGrain.GetCart(addItem.CartId);
    //   return cart;
    // });
  }
  // dotnet add package Microsoft.AspNetCore.App
}

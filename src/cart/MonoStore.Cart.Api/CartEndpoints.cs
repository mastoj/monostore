namespace MonoStore.Cart.Module;

using System.Diagnostics;
using System.Diagnostics.Metrics;
using OpenTelemetry.Metrics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Monostore.ServiceDefaults;
using MonoStore.Cart.Contracts;
using MonoStore.Cart.Contracts.Grains;
using Orleans;
using MonoStore.Product.Contracts.Grains;

public static class CartEndpoints
{
  public static Meter Meter => new Meter("MonoStore.Cart.Api");
  public static Counter<long> OperationsCounter => Meter.CreateCounter<long>("operations");

  // public static Counter<long> CartItemsCount => Meter.CreateCounter<long>("cart.items.add");
  // public static Histogram<long> CartValue => GetMeter(apiServiceName).CreateHistogram<long>("cart.value");


  public static string CartGrainId(Guid cartId) => $"cart/{cartId.ToString().ToLower()}";
  public static void MapCartEndpoints(this IEndpointRouteBuilder routes)
  {
    routes.MapPost("/", async (IGrainFactory grains, CreateCart createCart) =>
    {
      // DiagnosticConfig.CreateCartCounter.Add(1, new KeyValuePair<string, object?>("operatingChain", "OCNOELK"));
      try
      {
        Console.WriteLine("CreateCart");
        var cartGrain = grains.GetGrain<ICartGrain>(CartGrainId(createCart.CartId));
        var result = await cartGrain.CreateCart(createCart);
        OperationsCounter.Add(1, new TagList() {
          { "operation", "create" },
          { "status", "success" },
        });
        Console.WriteLine($"Cart created: {result.Id}");
        return result;
      }
      catch (Exception ex)
      {
        Console.WriteLine("Error creating cart");
        Console.WriteLine(ex.Message);
        OperationsCounter.Add(1, new TagList() {
          { "operation", "create" },
          { "status", "error" },
        });
        throw;
      }
    });

    routes.MapPost("/{id}/items", async (IGrainFactory grains, AddItemRequest addItemRequest) =>
    {
      var productGrainId = IProductGrain.ProductGrainId(addItemRequest.OperatingChain, addItemRequest.ProductId);
      Console.WriteLine($"==> ProductGrainId: {productGrainId}");
      var productGrain = grains.GetGrain<IProductGrain>(productGrainId);
      var product = await productGrain.GetProductAsync();
      var cartGrain = grains.GetGrain<ICartGrain>(CartGrainId(addItemRequest.CartId));
      if (product == null)
      {
        throw new Exception("Product not found");
      }
      if (product.Price == null)
      {
        throw new Exception("Product price not found");
      }
      var addItem = new AddItem
      {
        CartId = addItemRequest.CartId,
        Item = new CartItem
        {
          Product = new Product
          {
            Id = product.ArticleNumber,
            Name = product.Name ?? "",
            Price = product.Price.Price,
            PriceExVat = product.Price.PriceExclVat,
            Url = product.ProductUrl ?? "",
            PrimaryImageUrl = product.ImageUrl ?? ""
          },
          Quantity = 1
        }
      };
      return await cartGrain.AddItem(addItem);
    });

    routes.MapDelete("/{id}/items/{productId}", async (IGrainFactory grains, Guid id, string productId) =>
    {
      var cartGrain = grains.GetGrain<ICartGrain>(CartGrainId(id));
      return await cartGrain.RemoveItem(new RemoveItem
      {
        CartId = id,
        ProductId = productId
      });
    });

    routes.MapPost("/{id}/items/{productId}/increase", async (IGrainFactory grains, Guid id, string productId) =>
    {
      var cartGrain = grains.GetGrain<ICartGrain>(CartGrainId(id));
      return await cartGrain.IncreaseItemQuantity(new IncreaseItemQuantity
      {
        CartId = id,
        ProductId = productId
      });
    });

    routes.MapPost("/{id}/items/{productId}/decrease", async (IGrainFactory grains, Guid id, string productId) =>
    {
      var cartGrain = grains.GetGrain<ICartGrain>(CartGrainId(id));
      return await cartGrain.DecreaseItemQuantity(new DecreaseItemQuantity
      {
        CartId = id,
        ProductId = productId
      });
    });

    routes.MapGet("/{id}", async (IGrainFactory grains, Guid id) =>
    {
      try
      {
        OperationsCounter.Add(1, new TagList() {
          { "operation", "get" },
          { "status", "success" },
        });

        var cartGrain = grains.GetGrain<ICartGrain>(CartGrainId(id));
        return await cartGrain.GetCart(new GetCart { CartId = id });
      }
      catch (Exception ex)
      {
        OperationsCounter.Add(1, new TagList() {
          { "operation", "get" },
          { "status", "success" },
        });

        Console.WriteLine(ex.Message);
        throw;
      }
    });

    // routes.MapGet("/", async (IGrainFactory grains) =>
    // {
    //   var cartGrain = grains.GetGrain<ICartGrain>("cart");
    //   return await cartGrain.GetCart("1");
    // });

    // routes.MapGet("/{id}", async (IGrainFactory grains, int id) =>
    // {
    //   var cartGrain = grains.GetGrain<ICartGrain>("cart" + id);
    //   return await cartGrain.GetCart(id);
    // });

    // routes.MapPost("/", async (IGrainFactory grains, AddItem addItem) =>
    // {
    //   var cartGrain = grains.GetGrain<ICartGrain>("cart");
    //   var cart = await cartGrain.GetCart(addItem.CartId);
    //   return cart;
    // });
    // dotnet add package Microsoft.AspNetCore.App
  }

  public static WebApplication UseCart(this WebApplication app, string groupPath)
  {
    app.MapGroup(groupPath).MapCartEndpoints();
    return app;
  }
}
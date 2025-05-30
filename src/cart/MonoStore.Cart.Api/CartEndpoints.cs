namespace MonoStore.Cart.Api;

using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using MonoStore.Contracts.Cart.Grains;
using Orleans;
using MonoStore.Contracts.Product.Grains;
using MonoStore.Contracts.Cart.Requests;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using MonoStore.Cart.Domain;
using MonoStore.Contracts.Cart.Dtos;
using Monostore.Orleans.Types;
using global::Marten; // Add this line with global:: prefix


public static class CartEndpoints
{
  public static Meter Meter => new Meter("MonoStore.Cart.Api");
  public static Counter<long> OperationsCounter => Meter.CreateCounter<long>("operations");

  // public static Counter<long> CartItemsCount => Meter.CreateCounter<long>("cart.items.add");
  // public static Histogram<long> CartValue => GetMeter(apiServiceName).CreateHistogram<long>("cart.value");

  public static RouteGroupBuilder MapCartEndpoints(this RouteGroupBuilder routes)
  {

    routes.MapGet("/", async (ICartStore cartStore, string operatingChain, string? userId, string? sessionId, string? sku, string? query) =>
    {
      await using var session = cartStore.LightweightSession();
      var querySession = session.Query<Cart>().Where(c => c.OperatingChain == operatingChain);
      if (userId != null)
      {
        querySession = querySession.Where(c => c.UserId == userId);
      }
      if (sessionId != null)
      {
        querySession = querySession.Where(c => c.SessionId == sessionId);
      }
      if (sku != null)
      {
        querySession = querySession.Where(c => c.Items.Any(i => i.Product.Id == sku));
      }

      var carts = await querySession.ToListAsync();
      return Results.Ok(carts);
    }).Produces<List<Cart>>();

    routes.MapPost("/", async (HttpRequest request, IGrainFactory grains, CreateCartRequest createCart, ILoggerFactory loggerFactory) =>
    {
      var logger = loggerFactory.CreateLogger("CartEndpoints");
      if (!request.Cookies.TryGetValue("session-id", out var sessionId))
      {
        return Results.BadRequest("Missing session-id cookie");
      }

      request.Cookies.TryGetValue("user-id", out var userId);
      var cartId = Guid.NewGuid();
      // DiagnosticConfig.CreateCartCounter.Add(1, new KeyValuePair<string, object?>("operatingChain", "OCNOELK"));
      try
      {
        Console.WriteLine("CreateCart");
        var cartGrain = grains.GetGrain<ICartGrain>(ICartGrain.CartGrainId(cartId));
        var result = await cartGrain.CreateCart(new CreateCartMessage(cartId, createCart.OperatingChain, sessionId, userId));
        OperationsCounter.Add(1, new TagList() {
          { "operation", "create" },
          { "status", "success" },
        });
        return Results.Ok(result);
      }
      catch (Exception ex)
      {
        OperationsCounter.Add(1, new TagList() {
          { "operation", "create" },
          { "status", "failure" },
        });
        logger.LogError(ex, "Error creating cart");
        return Results.Problem(ex.Message);
      }
    }).Produces<GrainResult<CartData, CartError>>();

    routes.MapPost("/{id}/items", async (IGrainFactory grains, Guid id, AddItemRequest addItemRequest) =>
    {
      var productGrainId = IProductGrain.ProductGrainId(addItemRequest.OperatingChain, addItemRequest.ProductId);
      Console.WriteLine($"==> ProductGrainId: {productGrainId}");
      var cartGrain = grains.GetGrain<ICartGrain>(ICartGrain.CartGrainId(id));
      return await cartGrain.AddItem(addItemRequest);
    }).Produces<GrainResult<CartData, CartError>>();

    routes.MapDelete("/{id}/items/{productId}", async (IGrainFactory grains, Guid id, string productId) =>
    {
      var cartGrain = grains.GetGrain<ICartGrain>(ICartGrain.CartGrainId(id));
      return await cartGrain.RemoveItem(new RemoveItem(productId));
    }).Produces<GrainResult<CartData, CartError>>();

    routes.MapPut("/{id}/items/{productId}", async (IGrainFactory grains, Guid id, string productId, ChangeItemQuantity changeItemQuantity) =>
    {
      var cartGrain = grains.GetGrain<ICartGrain>(ICartGrain.CartGrainId(id));
      return await cartGrain.ChangeItemQuantity(new ChangeItemQuantity(productId, changeItemQuantity.Quantity));
    }).Produces<GrainResult<CartData, CartError>>();

    routes.MapGet("/{id}", async (IGrainFactory grains, Guid id) =>
    {
      try
      {
        OperationsCounter.Add(1, new TagList() {
          { "operation", "get" },
          { "status", "success" },
        });

        var cartGrain = grains.GetGrain<ICartGrain>(ICartGrain.CartGrainId(id));
        return await cartGrain.GetCart(new GetCart());
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
    }).Produces<GrainResult<CartData, CartError>>();

    routes.MapGet("/{id}/changes", async (ICartStore cartStore, Guid id) =>
    {
      await using var session = cartStore.LightweightSession();
      var changes = await session.Events.FetchStreamAsync(id);
      var result = changes?.Select(c => new Change(c.EventTypeName, c.Timestamp, c.Version, c.Data)).ToList();
      return Results.Ok(result);
    }).Produces<List<Change>>();

    routes.MapPost("/{id}/abandon", async (IGrainFactory grains, Guid id) =>
    {
      var cartGrain = grains.GetGrain<ICartGrain>(ICartGrain.CartGrainId(id));
      return await cartGrain.AbandonCart(new AbandonCart());
    }).Produces<GrainResult<CartData, CartError>>();

    routes.MapPost("/{id}/clear", async (IGrainFactory grains, Guid id) =>
    {
      var cartGrain = grains.GetGrain<ICartGrain>(ICartGrain.CartGrainId(id));
      return await cartGrain.ClearCart(new ClearCart());
    }).Produces<GrainResult<CartData, CartError>>();

    routes.MapPost("/{id}/recover", async (IGrainFactory grains, Guid id) =>
    {
      var cartGrain = grains.GetGrain<ICartGrain>(ICartGrain.CartGrainId(id));
      return await cartGrain.RecoverCart(new RecoverCart());
    }).Produces<GrainResult<CartData, CartError>>();

    routes.MapPost("/{id}/archive", async (IGrainFactory grains, Guid id) =>
    {
      var cartGrain = grains.GetGrain<ICartGrain>(ICartGrain.CartGrainId(id));
      return await cartGrain.ArchiveCart(new ArchiveCart());
    }).Produces<GrainResult<CartData, CartError>>();

    return routes;
  }

  public static T AddCart<T>(this T builder) where T : IHostApplicationBuilder
  {
    var connectionStringName = "monostorepg";
    var databaseSchemaName = "cart";
    var connectionString = $"{builder.Configuration.GetConnectionString(connectionStringName)};sslmode=prefer;CommandTimeout=300";

    // builder.Services.AddSingleton<ICartStore, ICartStore>();
    // builder.Services.AddSingleton<ICartGrain, CartGrain>();
    builder.Services.AddMartenStore<ICartStore>(s =>
    {
      var options = new StoreOptions();
      options.Events.MetadataConfig.CorrelationIdEnabled = true;
      options.Events.MetadataConfig.CausationIdEnabled = true;
      options.DatabaseSchemaName = databaseSchemaName;
      options.Connection(connectionString ?? throw new InvalidOperationException());
      // options.OpenTelemetry.TrackConnections = TrackLevel.Verbose;
      options.OpenTelemetry.TrackEventCounters();
      options.AutoCreateSchemaObjects = Weasel.Core.AutoCreate.None;
      return options;
    });

    return builder;
  }

  public static WebApplication UseCart(this WebApplication app, string groupPath)
  {
    app.MapGroup(groupPath).MapCartEndpoints().WithTags("cart");
    return app;
  }
}

public interface ICartStore : IDocumentStore { }
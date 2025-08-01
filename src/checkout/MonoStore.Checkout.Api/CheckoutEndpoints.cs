namespace MonoStore.Checkout.Api;

using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using MonoStore.Contracts.Cart.Grains;
using Orleans;
// using Microsoft.AspNetCore.Http;
using MonoStore.Contracts.Checkout.Requests;
using MonoStore.Contracts.Checkout;
using MonoStore.Contracts.Cart.Requests;
using global::Marten;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using MonoStore.Checkout.Domain;
using Monostore.Orleans.Types;
using MonoStore.Contracts.Checkout.Grains;

public static class CheckoutEndpoints
{
  public static Meter Meter => new Meter("MonoStore.Checkout.Api");
  public static Counter<long> OperationsCounter => Meter.CreateCounter<long>("operations");

  // public static Counter<long> CartItemsCount => Meter.CreateCounter<long>("cart.items.add");
  // public static Histogram<long> CartValue => GetMeter(apiServiceName).CreateHistogram<long>("cart.value");


  public static RouteGroupBuilder MapCheckoutEndpoints(this RouteGroupBuilder routes)
  {
    routes.MapPost("/", async (IGrainFactory grains, CreatePurchaseOrderRequest createPurchaseOrderRequest) =>
    {
      var purchaseOrderId = Guid.NewGuid();
      var cartGrain = grains.GetGrain<ICartGrain>(ICartGrain.CartGrainId(createPurchaseOrderRequest.CartId));
      var cartResult = await cartGrain.GetCart(new GetCart());
      if (cartResult.Error != null && cartResult.Data == null)
      {
        return Results.BadRequest(cartResult.Error.Type);

      }
      var cart = cartResult.Data!;
      var orderItems = cart.Items.Select(i =>
      {
        var product = new Product(i.Product.Id, i.Product.Name, i.Product.Price, i.Product.PriceExVat, i.Product.Url, i.Product.PrimaryImageUrl);
        return new PurchaseOrderItem(product, i.Quantity);
      }).ToArray();
      var createPurchaseOrderMessage = new CreatePurchaseOrderMessage(purchaseOrderId, createPurchaseOrderRequest.CartId, cart.OperatingChain, cart.SessionId, cart.UserId, orderItems);
      Console.WriteLine("CreatePurchaseOrder: " + System.Text.Json.JsonSerializer.Serialize(createPurchaseOrderMessage));

      var purchaseOrderGrain = grains.GetGrain<IPurchaseOrderGrain>(IPurchaseOrderGrain.PurchaseOrderGrainId(purchaseOrderId));
      var result = await purchaseOrderGrain.CreatePurchaseOrder(createPurchaseOrderMessage);

      return Results.Ok(result);
      // DiagnosticConfig.CreateCartCounter.Add(1, new KeyValuePair<string, object?>("operatingChain", "OCNOELK"));
    }).Produces<GrainResult<PurchaseOrderData, CheckoutError>>();

    routes.MapGet("/", async (ICheckoutStore checkoutStore, string operatingChain, string? userId, string? sessionId, string? sku, string? cartId, string? query, int? page = 1, int? pageSize = 50) =>
    {
      await using var session = checkoutStore.LightweightSession();
      var querySession = session.Query<PurchaseOrder>().Where(c => c.OperatingChain == operatingChain);
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
      if (cartId != null)
      {
        querySession = querySession.Where(c => c.CartId == Guid.Parse(cartId));
      }

      // Apply pagination
      var currentPage = Math.Max(page ?? 1, 1); // Ensure page is at least 1
      var size = Math.Max(pageSize ?? 50, 1); // Ensure pageSize is at least 1
      var skip = (currentPage - 1) * size;

      // Get total count for pagination metadata
      var totalCount = await querySession.CountAsync();

      // Apply pagination to query
      var paginatedQuery = querySession.Skip(skip).Take(size);

      // Get the paginated results
      var purchaseOrders = await paginatedQuery.ToListAsync();

      // Return paginated results with pagination metadata
      return Results.Ok(new
      {
        data = purchaseOrders,
        pagination = new
        {
          currentPage,
          pageSize = size,
          totalItems = totalCount,
          totalPages = (int)Math.Ceiling((double)totalCount / size)
        }
      });
    }).Produces<object>();

    routes.MapGet("/{id}", async (IGrainFactory grainFactory, Guid id) =>
    {
      var purchaseOrderGrain = grainFactory.GetGrain<IPurchaseOrderGrain>(IPurchaseOrderGrain.PurchaseOrderGrainId(id));
      var result = await purchaseOrderGrain.GetPurchaseOrder(new GetPurchaseOrder());
      return Results.Ok(result);
    }).Produces<GrainResult<PurchaseOrderData, CheckoutError>>();

    routes.MapGet("/{id}/changes", async (ICheckoutStore checkoutStore, Guid id) =>
    {
      await using var session = checkoutStore.LightweightSession();
      var changes = await session.Events.FetchStreamAsync(id);
      var result = changes?.Select(c => new Change(c.EventTypeName, c.Timestamp, c.Version, c.Data)).ToList();
      return Results.Ok(result);
    }).Produces<List<Change>>();

    routes.MapPost("/{id}/payment", async (IGrainFactory grains, Guid id, AddPaymentRequest addPaymentRequest) =>
    {
      var addPaymentMessage = new AddPaymentMessage(
        addPaymentRequest.TransactionId,
        addPaymentRequest.PaymentMethod,
        addPaymentRequest.PaymentProvider,
        addPaymentRequest.Amount,
        addPaymentRequest.Currency,
        addPaymentRequest.ProcessedAt,
        addPaymentRequest.Status
      );

      var purchaseOrderGrain = grains.GetGrain<IPurchaseOrderGrain>(IPurchaseOrderGrain.PurchaseOrderGrainId(id));
      var result = await purchaseOrderGrain.AddPayment(addPaymentMessage);
      return Results.Ok(result);
    }).Produces<GrainResult<PurchaseOrderData, CheckoutError>>();

    return routes;
  }

  public static T AddCheckout<T>(this T builder) where T : IHostApplicationBuilder
  {
    var connectionStringName = "monostorepg";
    var databaseSchemaName = "monostore";
    var connectionString = $"{builder.Configuration.GetConnectionString(connectionStringName)};sslmode=prefer;CommandTimeout=300";

    builder.Services.AddMartenStore<ICheckoutStore>(s =>
    {
      var options = new StoreOptions();
      options.Events.MetadataConfig.CorrelationIdEnabled = true;
      options.Events.MetadataConfig.CausationIdEnabled = true;
      options.DatabaseSchemaName = databaseSchemaName;
      options.Connection(connectionString ?? throw new InvalidOperationException());
      options.OpenTelemetry.TrackEventCounters();
      options.AutoCreateSchemaObjects = JasperFx.AutoCreate.None;
      return options;
    });

    return builder;
  }

  public static WebApplication UseCheckout(this WebApplication app, string groupPath)
  {
    app.MapGroup(groupPath).MapCheckoutEndpoints().WithTags("checkout");
    return app;
  }
}

public interface ICheckoutStore : IDocumentStore { }
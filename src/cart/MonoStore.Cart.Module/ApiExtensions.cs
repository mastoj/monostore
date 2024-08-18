using Marten;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MonoStore.Marten;

namespace MonoStore.Cart.Module;

// public class DummyEventStore : IEventStore
// {
//   private static Dictionary<Guid, List<object>> _streams = new();
//   public Task<TState> AppendToStream<T, TState>(Guid id, T @event, int version, Func<T, TState> apply, CancellationToken ct) where T : class
//   {

//     if (!_streams.ContainsKey(id))
//     {
//       _streams[id] = new List<object>();
//     }

//     _streams[id].Add(@event);
//     return Task.FromResult(apply(@event));
//   }

//   public Task<TState> CreateStream<T, TState>(Guid id, T @event, Func<T, TState> create, CancellationToken ct) where T : class
//   {
//     if (!_streams.ContainsKey(id))
//     {
//       _streams[id] = new List<object>();
//     }

//     _streams[id].Add(@event);
//     return Task.FromResult(create(@event));
//   }
// }

public class MartenEventStore : IEventStore
{
  private IDocumentSession _session;
  public MartenEventStore(IDocumentSession session)
  {
    _session = session;
  }

  public async Task<TState> AppendToStream<T, TState>(Guid id, T @event, int version, Func<T, TState> apply, CancellationToken ct) where T : class
  {
    var state = apply(@event);
    _session.Events.Append(id, @event, ct);
    await _session.SaveChangesAsync(token: ct);
    return state;
  }

  public async Task<TState> CreateStream<T, TState>(Guid id, T @event, Func<T, TState> create, CancellationToken ct) where T : class
  {
    var state = create(@event);
    _session.Events.StartStream<T>(id, @event);
    await _session.SaveChangesAsync(token: ct);
    return state;
  }

  public async Task<TState> GetState<TState>(Guid id, CancellationToken ct) where TState : class
  {
    var result = await _session.Events.AggregateStreamAsync<TState>(id, token: ct);
    return result;
  }
}

public static class ApiExtensions
{
  public static WebApplicationBuilder AddCart(this WebApplicationBuilder builder)
  {
    // Try three times
    Action doStuff = () =>
    {

      var connectionString = $"{builder.Configuration.GetConnectionString("cart")};sslmode=prefer;CommandTimeout=300";
      // var connectionString = "Host=localhost;Port=65356;Username=postgres;Password=Rvb+Zm5sR4kCUDHTvJZ8!V;sslmode=prefer"; //builder.Configuration.GetConnectionString("cart");
      // Thread.Sleep(4000);
      Console.WriteLine($"Cart ConnectionString: {connectionString}, schema: {Environment.GetEnvironmentVariable("SchemaName")}");
      builder.Services
        .AddTransient<IEventStore, MartenEventStore>();
      builder.Services.AddMarten(s =>
        {
          var options = new StoreOptions();
          // var schemaName = Environment.GetEnvironmentVariable("SchemaName") ?? "public";
          // options.Events.DatabaseSchemaName = schemaName;
          // options.DatabaseSchemaName = schemaName;
          options.Connection(connectionString ?? throw new InvalidOperationException());
          return options;
        })
        // .UseNpgsqlDataSource(s => {
        //   s.
        // })
        .UseLightweightSessions()
        .ApplyAllDatabaseChangesOnStartup();
    };
    var attempts = 0;
    var maxAttempts = 3;
    while (attempts < maxAttempts)
    {
      try
      {
        Console.WriteLine("==> Attempting to add cart");
        doStuff();
        break;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Attempt {attempts + 1} failed: {ex.Message}");
        attempts++;
        // Sleep 1 second
        Thread.Sleep(1000);
        if (attempts == maxAttempts)
        {
          throw;
        }
      }
    }
    return builder;
  }


  public static WebApplication UseCart(this WebApplication app, string groupPath)
  {
    app.MapGroup(groupPath).MapCartEndpoints();
    return app;
  }
}

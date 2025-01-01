using Marten;
using Marten.Services;
using Microsoft.AspNetCore.Builder;

namespace MonoStore.Cart.Module;

public class MartenEventStore : IEventStore
{
  private IDocumentStore _documentStore;
  public MartenEventStore(IDocumentStore documentStore)
  {
    _documentStore = documentStore;
  }

  public async Task<TState> AppendToStream<T, TState>(Guid id, T @event, int version, Func<T, TState> apply, CancellationToken ct) where T : class
  {
    using (var session = _documentStore.LightweightSession())
    {
      var state = apply(@event);
      Console.WriteLine($"Appending event {id} to stream: " + @event);
      session.Events.Append(id, version + 1, @event);
      await session.SaveChangesAsync(token: ct);
      return state;
    }
  }

  public async Task<TState> CreateStream<T, TState>(Guid id, T @event, Func<T, TState> create, CancellationToken ct) where T : class
  {
    using (var session = _documentStore.LightweightSession())
    {
      var state = create(@event);
      session.Events.StartStream<T>(id, @event);
      await session.SaveChangesAsync(token: ct);
      return state;
    }
  }

  public async Task<TState> GetState<TState>(Guid id, CancellationToken ct) where TState : class
  {
    using var _session = _documentStore.QuerySession();
    var result = await _session.Events.AggregateStreamAsync<TState>(id, token: ct);
    return result;
  }
}

public static class ApiExtensions
{
  public static WebApplicationBuilder AddCart(this WebApplicationBuilder builder)
  {
    // Try three times


    return builder;
  }
}

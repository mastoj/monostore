using Marten;
using System.Collections.Concurrent;

namespace MonoStore.Marten;
public interface IEventStore
{
  Task<TState> CreateStream<T, TState>(Guid id, T @event, Func<T, TState> apply, CancellationToken ct) where T : class;
  Task<TState> AppendToStream<T, TState>(Guid id, T @event, int version, Func<T, TState> apply, CancellationToken ct) where T : class;
  Task<TState> GetState<TState>(Guid id, CancellationToken ct) where TState : class;
}

public class MartenEventStore : IEventStore
{
  private readonly IDocumentStore _documentStore;

  // Global throttle to limit the total number of concurrent database operations
  private static readonly SemaphoreSlim _writeOperationsThrottle = new SemaphoreSlim(20, 20);
  private static readonly SemaphoreSlim _readOperationsThrottle = new SemaphoreSlim(40, 40);

  public MartenEventStore(IDocumentStore documentStore)
  {
    _documentStore = documentStore;
  }

  public async Task<TState> AppendToStream<T, TState>(Guid id, T @event, int version, Func<T, TState> apply, CancellationToken ct) where T : class
  {
    // Acquire a ticket from the throttle before accessing the database
    await _writeOperationsThrottle.WaitAsync(ct);
    try
    {
      await using var session = _documentStore.LightweightSession();
      var state = apply(@event);
      session.Events.Append(id, version, @event);
      await session.SaveChangesAsync(token: ct);
      return state;
    }
    finally
    {
      _writeOperationsThrottle.Release();
    }
  }

  public async Task<TState> CreateStream<T, TState>(Guid id, T @event, Func<T, TState> create, CancellationToken ct) where T : class
  {
    await _writeOperationsThrottle.WaitAsync(ct);
    try
    {
      await using var session = _documentStore.LightweightSession();
      var state = create(@event);
      session.Events.StartStream<T>(id, @event);
      await session.SaveChangesAsync(token: ct);
      return state;
    }
    finally
    {
      _writeOperationsThrottle.Release();
    }
  }

  public async Task<TState> GetState<TState>(Guid id, CancellationToken ct) where TState : class
  {
    await _readOperationsThrottle.WaitAsync(ct);
    try
    {
      await using var session = _documentStore.QuerySession();
      var result = await session.Events.AggregateStreamAsync<TState>(id, token: ct);
      return result;
    }
    finally
    {
      _readOperationsThrottle.Release();
    }
  }
}

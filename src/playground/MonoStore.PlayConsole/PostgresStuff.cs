using System.Diagnostics;
using Npgsql;
using Dapper;
using System.Text.Json.Nodes;

namespace MonoStore.PlayConsole;

// Create steram type consisting of id (guid)

public static class PostgresStuff
{
  public static void DoStuff()
  {
    var connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING");
    // Start timer to measure how long it takes to execute the an insert query.
    // For each insert should insert a new random event into the database and commit it.
    // Insert 10000 events and measure the time it takes to insert them.

    var events = new List<Event>();
    var randomJsonObject = @"{""prop1"": ""value1""}";
    //, prop2: ""value2"", prop3: ""value3"", prop4: ""value4"", prop5: ""value5"", prop6: ""value6"", prop7: ""value7"", prop8: ""value8"", prop9: ""value9"", prop10: ""value10"", prop11: ""value11"", prop12: ""value12"", prop13: ""value13"", prop14: ""value14"", prop15: ""value15"", prop16: ""value16"", prop17: ""value17"", prop18: ""value18"", prop19: ""value19"", prop20: ""value20""}";
    for (int i = 0; i < 1000; i++)
    {
      events.Add(new Event
      {
        Id = Guid.NewGuid(),
        StreamId = Guid.NewGuid(),
        Version = i,
        Data = randomJsonObject,
        Type = "TestEvent",
        Timestamp = DateTimeOffset.UtcNow,
        TenantId = "test",
        MtDotnetType = "MonoStore.PlayConsole.Event",
        CorrelationId = null,
        CausationId = null,
        IsArchived = false
      });
    }

    // Insert the stremas before starting the timer
    using (var connection = new NpgsqlConnection(connectionString))
    {
      connection.Open();
      foreach (var @event in events)
      {
        connection.Execute(
          @"INSERT INTO monostore.mt_streams (id) VALUES (@Id)",
          new Stream { Id = @event.StreamId });
      }
    }

    var stopwatch = new Stopwatch();
    stopwatch.Start();
    using (var connection = new NpgsqlConnection(connectionString))
    {
      connection.Open();
      foreach (var @event in events)
      {
        connection.Execute(
          @"INSERT INTO monostore.mt_events (seq_id, id, stream_id, version, data, type, timestamp, tenant_id, mt_dotnet_type, correlation_id, causation_id, is_archived) VALUES (nextval('monostore.mt_events_sequence'), @Id, @StreamId, @Version, @Data::jsonb, @Type, @Timestamp, @TenantId, @MtDotnetType, @CorrelationId, @CausationId, @IsArchived)",
          @event);
      }
    }
    stopwatch.Stop();
    Console.WriteLine($"Inserting 1000 events took {stopwatch.ElapsedMilliseconds} ms");
    // Ms per event
    Console.WriteLine($"Inserting 1000 events took {stopwatch.ElapsedMilliseconds / 1000} ms per event");
    // Events per second
    Console.WriteLine($"Inserting 1000 events took {1000 / (stopwatch.ElapsedMilliseconds / 1000)} events per second");
  }


  public static void DoStuffParallel()
  {
    var connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING");
    // Start timer to measure how long it takes to execute the an insert query.
    // For each insert should insert a new random event into the database and commit it.
    // Insert 10000 events and measure the time it takes to insert them.

    var events = new List<Event>();
    var randomJsonObject = @"{""prop1"": ""value1""}";
    //, prop2: ""value2"", prop3: ""value3"", prop4: ""value4"", prop5: ""value5"", prop6: ""value6"", prop7: ""value7"", prop8: ""value8"", prop9: ""value9"", prop10: ""value10"", prop11: ""value11"", prop12: ""value12"", prop13: ""value13"", prop14: ""value14"", prop15: ""value15"", prop16: ""value16"", prop17: ""value17"", prop18: ""value18"", prop19: ""value19"", prop20: ""value20""}";
    var eventCount = 5000;
    for (int i = 0; i < 5000; i++)
    {
      events.Add(new Event
      {
        Id = Guid.NewGuid(),
        StreamId = Guid.NewGuid(),
        Version = i,
        Data = randomJsonObject,
        Type = "TestEvent",
        Timestamp = DateTimeOffset.UtcNow,
        TenantId = "test",
        MtDotnetType = "MonoStore.PlayConsole.Event",
        CorrelationId = null,
        CausationId = null,
        IsArchived = false
      });
    }

    // Insert the stream ids in a batch insert
    using (var connection = new NpgsqlConnection(connectionString))
    {
      Console.WriteLine("Inserting streams");
      connection.Open();
      connection.Execute(
        @"INSERT INTO monostore.mt_streams (id) VALUES (@Id)",
        events.Select(@event => new Stream { Id = @event.StreamId }));
    }

    Console.WriteLine("Starting parallel insert");
    var stopwatch = new Stopwatch();
    stopwatch.Start();
    Parallel.ForEach(events, @event =>
    {
      using (var connection = new NpgsqlConnection(connectionString))
      {
        connection.Open();
        connection.Execute(
          @"INSERT INTO monostore.mt_events (seq_id, id, stream_id, version, data, type, timestamp, tenant_id, mt_dotnet_type, correlation_id, causation_id, is_archived) VALUES (nextval('monostore.mt_events_sequence'), @Id, @StreamId, @Version, @Data::jsonb, @Type, @Timestamp, @TenantId, @MtDotnetType, @CorrelationId, @CausationId, @IsArchived)",
          @event);
      }
    });
    stopwatch.Stop();
    Console.WriteLine($"Inserting {eventCount} events took {stopwatch.ElapsedMilliseconds} ms");
    // Events per second
    Console.WriteLine($"Inserting {eventCount} events took {eventCount / (stopwatch.ElapsedMilliseconds / 1000)} events per second");
  }
}
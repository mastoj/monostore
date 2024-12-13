using System.Diagnostics;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MonoStore.PlayConsole;

public static class TableStorageStuff
{
  private const int EventCount = 5000; // Example event count

  public static void DoStuff()
  {
    // Create a table client
    // Create a table if it does not exist
    // Insert 10000 entities into the table
    // Measure the time it takes to insert the entities

    var connectionString = Environment.GetEnvironmentVariable("AZURE_TABLE_STORAGE_CONNECTION_STRING");
    var tableName = "events";
    var tableClient = new TableClient(connectionString, tableName);

    // Create the table if it does not exist
    tableClient.CreateIfNotExists();

    var stopwatch = new Stopwatch();
    stopwatch.Start();
    for (int i = 0; i < EventCount; i++)
    {
      // Data should be a json string with 8 properties
      var data = @"{""prop1"": ""value1"", ""prop2"": ""value2"", ""prop3"": ""value3"", ""prop4"": ""value4"", ""prop5"": ""value5"", ""prop6"": ""value6"", ""prop7"": ""value7"", ""prop8"": ""value8""}";
      var metadata = @"{""correlationId"": null, ""causationId"": null}";
      var entity = new TableEntity(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
      {
        { "data", data },
        { "meta", metadata},
        { "type", "TestEvent" },
        { "timestamp", DateTimeOffset.UtcNow },
        { "tenantId", "test" },
        { "mtDotnetType", "MonoStore.PlayConsole.Event" },
        { "correlationId", null },
        { "causationId", null },
        { "isArchived", false }
      };
      tableClient.AddEntity(entity);
    }
    stopwatch.Stop();
    Console.WriteLine($"Time to insert {EventCount} entities: {stopwatch.ElapsedMilliseconds} ms");
    Console.WriteLine($"Time entities per second {EventCount / (stopwatch.ElapsedMilliseconds / 1000)}");
  }

  public static void DoStuffParallel()
  {
    // Create a table client
    // Create a table if it does not exist
    // Insert 10000 entities into the table
    // Measure the time it takes to insert the entities

    var connectionString = Environment.GetEnvironmentVariable("AZURE_TABLE_STORAGE_CONNECTION_STRING");
    var tableName = "events";
    var tableClient = new TableClient(connectionString, tableName);

    // Create the table if it does not exist
    tableClient.CreateIfNotExists();

    var stopwatch = new Stopwatch();
    stopwatch.Start();
    Parallel.For(0, EventCount, i =>
    {
      // Data should be a json string with 8 properties
      var data = @"{""prop1"": ""value1"", ""prop2"": ""value2"", ""prop3"": ""value3"", ""prop4"": ""value4"", ""prop5"": ""value5"", ""prop6"": ""value6"", ""prop7"": ""value7"", ""prop8"": ""value8""}";
      var metadata = @"{""correlationId"": null, ""causationId"": null}";
      var entity = new TableEntity(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
      {
        { "data", data },
        { "meta", metadata},
        { "type", "TestEvent" },
        { "timestamp", DateTimeOffset.UtcNow },
        { "tenantId", "test" },
        { "mtDotnetType", "MonoStore.PlayConsole.Event" },
        { "correlationId", null },
        { "causationId", null },
        { "isArchived", false }
      };
      tableClient.AddEntity(entity);
    });
    stopwatch.Stop();
    Console.WriteLine($"Time to insert {EventCount} entities: {stopwatch.ElapsedMilliseconds} ms");
    Console.WriteLine($"Time entities per second {EventCount / (stopwatch.ElapsedMilliseconds / 1000)}");
  }

  public static async Task InsertEntitiesAsync()
  {
    var connectionString = Environment.GetEnvironmentVariable("AZURE_TABLE_STORAGE_CONNECTION_STRING");
    var tableName = "events";
    var tableClient = new TableClient(connectionString, tableName);

    // Create the table if it does not exist
    tableClient.CreateIfNotExists();

    var stopwatch = new Stopwatch();
    stopwatch.Start();

    var tasks = new List<Task>();

    for (int i = 0; i < EventCount; i++)
    {
      tasks.Add(Task.Run(async () =>
      {
        // Data should be a json string with 8 properties
        var data = @"{""prop1"": ""value1"", ""prop2"": ""value2"", ""prop3"": ""value3"", ""prop4"": ""value4"", ""prop5"": ""value5"", ""prop6"": ""value6"", ""prop7"": ""value7"", ""prop8"": ""value8""}";
        var metadata = @"{""correlationId"": null, ""causationId"": null}";
        var entity = new TableEntity(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
        {
          { "data", data },
          { "meta", metadata }
        };

        await tableClient.AddEntityAsync(entity);
      }));
    }

    await Task.WhenAll(tasks);

    stopwatch.Stop();
    Console.WriteLine($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
    Console.WriteLine($"Entities per second: {EventCount / (stopwatch.ElapsedMilliseconds / 1000)}");
  }
}

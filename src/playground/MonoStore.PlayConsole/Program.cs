// See https://aka.ms/new-console-template for more information
using dotenv.net;
using Microsoft.Azure.Cosmos;
using MonoStore.PlayConsole;

DotEnv.Load();

// Test the DumpProductsToFile method
var cosmosConnectionString = Environment.GetEnvironmentVariable("COSMOS_CONNECTION_STRING");
var cosmosClient = new CosmosClient(cosmosConnectionString);

var repository = new ProductRepository(cosmosClient);
await repository.DumpProductsToFile();

Console.WriteLine("==> DumpProductsToFile test completed");

// var product = await repository.GetProductAsync("209138", "OCSEELG");
// Console.WriteLine(product);
// PostgresStuff.DoStuff();
// PostgresStuff.DoStuffParallel();
// TableStorageStuff.DoStuffParallel();
// TableStorageStuff.InsertEntitiesAsync().Wait();
// See https://aka.ms/new-console-template for more information
using System.Reflection;
using DbUp;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Hello, World!");

try
{

  var configuration = new ConfigurationBuilder()
        .AddEnvironmentVariables()
        .Build();

  var connectionString = $"{configuration["ConnectionStrings:monostorepg"]};Database=postgres";
  EnsureDatabase.For.PostgresqlDatabase(connectionString);

  var upgrader =
      DeployChanges.To
          .PostgresqlDatabase(connectionString)
          .WithVariablesDisabled()
          .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
          .LogToConsole()
          .Build();

  var result = upgrader.PerformUpgrade();

  if (!result.Successful)
  {
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(result.Error);
    Console.ResetColor();
#if DEBUG
    Console.ReadLine();
#endif
    return -1;
  }

  Console.ForegroundColor = ConsoleColor.Green;
  Console.WriteLine("Success!");
  Console.ResetColor();
  return 0;
}
catch (Exception ex)
{
  Console.ForegroundColor = ConsoleColor.Red;
  Console.WriteLine(ex.Message);
  Console.ResetColor();
  return -1;
}

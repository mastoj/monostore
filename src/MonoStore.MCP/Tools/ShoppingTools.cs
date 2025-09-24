using System.ComponentModel;
using System.Net.Http.Json;
using ModelContextProtocol.Server;
using MonoStore.Contracts.Cart.Requests;

namespace MonoStore.MCP.Tools;

[McpServerToolType]
public class ShoppingTools
{
  [McpServerTool, Description("Create a new cart to use for shopping.")]
  public static async Task<string> CreateCart()
  {
    var createCartRequest = new CreateCartRequest("OCSEELG");
    var cookies = new Dictionary<string, string>();
    cookies["session-id"] = "54321";
    cookies["user-id"] = "12345";
    // localhost:5170/cart
    var httpClient = new HttpClient();
    httpClient.DefaultRequestHeaders.Add("Cookie", $"session-id={cookies["session-id"]}; user-id={cookies["user-id"]}");
    var response = await httpClient.PostAsJsonAsync("http://localhost:5170/cart", createCartRequest);
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadAsStringAsync();
  }
  [McpServerTool, Description("Adds an item to the cart using the provided sku.")]
  public static string AddItemToCart([Description("The ID of the cart to add the item to")] string cartId, [Description("The SKU of the item to add to the cart")] string sku)
  {
    var addItemRequest = new AddItemRequest("OCSEELG", sku);
    var cookies = new Dictionary<string, string>();
    cookies["session-id"] = "54321";
    cookies["user-id"] = "12345";
    // localhost:5170/cart/{cartId}/items
    var httpClient = new HttpClient();
    var response = httpClient.PostAsJsonAsync($"http://localhost:5170/cart/{cartId}/items", addItemRequest).Result;
    response.EnsureSuccessStatusCode();
    return response.Content.ReadAsStringAsync().Result;
  }

  [McpServerTool, Description("Increase quantity of item in cart.")]
  public static string ReverseEcho(string message) => new string(message.Reverse().ToArray());

}

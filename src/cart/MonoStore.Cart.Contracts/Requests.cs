using MonoStore.Cart.Contracts.Dtos;

namespace MonoStore.Cart.Contracts.Requests;

[GenerateSerializer]
public record CreateCart(Guid CartId, string OperatingChain);

[GenerateSerializer]
public record GetCart;

[GenerateSerializer]
public record AddItemRequest(string OperatingChain, string ProductId);

[GenerateSerializer]
public record RemoveItem(string ProductId);

[GenerateSerializer]
public record ChangeItemQuantity(string ProductId, int Quantity);

[GenerateSerializer]
public record AbandonCart;

[GenerateSerializer]
public record RecoverCart;

[GenerateSerializer]
public record ArchiveCart;

[GenerateSerializer]
public record ClearCart;

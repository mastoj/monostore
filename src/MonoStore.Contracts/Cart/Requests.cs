using MonoStore.Contracts.Cart.Dtos;

namespace MonoStore.Contracts.Cart.Requests;

[GenerateSerializer]
public record CreateCartRequest(string OperatingChain);
[GenerateSerializer]
public record CreateCartMessage(Guid CartId, string OperatingChain, string SessionId, string? UserId);
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

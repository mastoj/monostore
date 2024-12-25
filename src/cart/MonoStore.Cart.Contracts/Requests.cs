using MonoStore.Cart.Contracts.Dtos;

namespace MonoStore.Cart.Contracts.Requests;

[GenerateSerializer, Alias(nameof(CreateCart))]
public record class CreateCart
{
  [Id(0)]
  public Guid CartId { get; set; } = Guid.Empty;
  [Id(1)]
  public string OperatingChain { get; set; } = "";
}

[GenerateSerializer, Alias(nameof(GetCart))]
public record class GetCart
{
}

[GenerateSerializer, Alias(nameof(AddItemRequest))]
public record AddItemRequest
{
  [Id(0)]
  public string OperatingChain { get; set; } = "";
  [Id(1)]
  public string ProductId { get; set; } = "";
}

[GenerateSerializer, Alias(nameof(RemoveItem))]
public record class RemoveItem
{
  [Id(0)]
  public string ProductId { get; set; } = "";
}
[GenerateSerializer, Alias(nameof(IncreaseItemQuantity))]
public record class IncreaseItemQuantity
{
  [Id(0)]
  public string ProductId { get; set; } = "";
}
[GenerateSerializer, Alias(nameof(DecreaseItemQuantity))]
public record class DecreaseItemQuantity
{
  [Id(0)]
  public string ProductId { get; set; } = "";
}

[GenerateSerializer, Alias(nameof(AbandonCart))]
public record class AbandonCart
{
}

[GenerateSerializer, Alias(nameof(RecoverCart))]
public record class RecoverCart
{
}

[GenerateSerializer, Alias(nameof(ArchiveCart))]
public record class ArchiveCart
{
}

[GenerateSerializer, Alias(nameof(ClearCart))]
public record class ClearCart
{
}

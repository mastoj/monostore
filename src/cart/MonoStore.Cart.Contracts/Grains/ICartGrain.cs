using MonoStore.Cart.Contracts.Requests;
using MonoStore.Cart.Contracts.Dtos;
using Monostore.Orleans.Types;

namespace MonoStore.Cart.Contracts.Grains;

public enum CartErrorType
{
  Unkown = 99
}

[GenerateSerializer, Alias(nameof(CartError))]
public record class CartError
{
  [Id(0)]
  public string Message { get; set; } = "";
  [Id(1)]
  public CartErrorType Type { get; set; } = CartErrorType.Unkown;
}

public interface ICartGrain : IGrainWithStringKey
{
  public static string CartGrainId(Guid cartId) => $"cart/{cartId.ToString().ToLower()}";

  Task<GrainResult<CartData, CartError>> CreateCart(CreateCartMessage createCart);
  Task<GrainResult<CartData, CartError>> GetCart(GetCart getCart);
  Task<GrainResult<CartData, CartError>> AddItem(AddItemRequest addItem);
  Task<GrainResult<CartData, CartError>> RemoveItem(RemoveItem removeItem);
  Task<GrainResult<CartData, CartError>> ChangeItemQuantity(ChangeItemQuantity changeItemQuantity);
  Task<GrainResult<CartData, CartError>> ClearCart(ClearCart clearCart);
  Task<GrainResult<CartData, CartError>> AbandonCart(AbandonCart abandonCart);
  Task<GrainResult<CartData, CartError>> RecoverCart(RecoverCart recoverCart);
  Task<GrainResult<CartData, CartError>> ArchiveCart(ArchiveCart archiveCart);
}
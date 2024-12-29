using MonoStore.Cart.Contracts.Requests;
using MonoStore.Cart.Contracts.Dtos;

namespace MonoStore.Cart.Contracts.Grains;


[GenerateSerializer, Alias("GrainResult`2")]
public record class GrainResult<T, E>
{
  [Id(0)]
  public T? Data { get; set; }
  [Id(1)]
  public E? Error { get; set; }

  public static GrainResult<T, E> Success(T result) => new GrainResult<T, E> { Data = result };
  public static GrainResult<T, E> Failure(E error) => new GrainResult<T, E> { Error = error };
}


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
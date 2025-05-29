using MonoStore.Contracts.Cart.Dtos;

namespace MonoStore.Contracts.Cart.Exceptions;

public class InvalidCartStatusException : Exception
{
  public CartStatus ExpectedStatus { get; }
  public CartStatus ActualStatus { get; }
  public InvalidCartStatusException(string operation, CartStatus expectedStatus, CartStatus actualStatus) : base($"[{operation}] Expected status {expectedStatus} but was {actualStatus}")
  {
    ExpectedStatus = expectedStatus;
    ActualStatus = actualStatus;
  }
}

using MonoStore.Cart.Contracts.Dtos;

namespace MonoStore.Cart.Module;

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

public class PaymentAlreadyAddedException : Exception
{
  public PaymentAlreadyAddedException(string operation) : base($"[{operation}] Payment has already been added to this purchase order.")
  {
  }
}

public class PaymentAmountMismatchException : Exception
{
  public PaymentAmountMismatchException(string operation, decimal expectedAmount, decimal actualAmount)
      : base($"[{operation}] Payment amount {actualAmount} does not match expected amount {expectedAmount}.")
  {
  }
}

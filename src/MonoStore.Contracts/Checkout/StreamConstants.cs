namespace MonoStore.Contracts.Checkout;

public static class StreamConstants
{
  public static readonly Guid ReportingGrainStreamGuid = new("11111111-1111-1111-1111-111111111111");
  public const string OrderPaidEventNamespace = "OrderPaidEvent";
  public const string OrderStreamProviderName = "OrderStreamProvider";
}
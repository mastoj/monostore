using Marten;
using Marten.Events.Projections;

namespace MonoStore.Checkout.Domain;

public static class CheckoutProjections
{
  public static StoreOptions AddCheckoutProjections(this StoreOptions storeOptions)
  {
    storeOptions.Projections.Snapshot<PurchaseOrder>(SnapshotLifecycle.Inline);
    return storeOptions;
  }
}
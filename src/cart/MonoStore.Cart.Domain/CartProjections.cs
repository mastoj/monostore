using Marten;
using Marten.Events.Projections;

namespace MonoStore.Cart.Domain;

public static class CartProjections
{
  public static StoreOptions AddCartProjections(this StoreOptions storeOptions)
  {
    storeOptions.Projections.Snapshot<Cart>(SnapshotLifecycle.Inline);
    return storeOptions;
  }
}
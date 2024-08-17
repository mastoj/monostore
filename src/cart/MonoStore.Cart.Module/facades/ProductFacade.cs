using DotNext;

namespace MonoStore.Cart.Module.Facades;

internal record Product(string id, string name, decimal price, decimal priceExVat);

interface IProductFacade
{
  Task<Result<Product>> GetProduct(Guid productId);
}
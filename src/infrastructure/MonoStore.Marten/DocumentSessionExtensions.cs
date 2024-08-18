using Marten;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MonoStore.Marten;

public static class DocumentSessionExtensions
{
  public static Task<T?> Get<T>(this IDocumentSession session, Guid id, CancellationToken ct) where T : class
  {
    return session.LoadAsync<T>(id, ct);
  }

  public static Task Add<T>(this IDocumentSession session, T entity, CancellationToken ct) where T : class
  {
    session.Store(entity);
    return session.SaveChangesAsync(ct);
  }
}

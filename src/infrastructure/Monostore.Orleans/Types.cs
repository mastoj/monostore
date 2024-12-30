namespace Monostore.Orleans.Types;


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

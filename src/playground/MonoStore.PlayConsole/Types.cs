namespace MonoStore.PlayConsole;

public class Stream
{
  public Guid Id { get; set; }
}


// Create a class called event that I can use together with dapper
// The fields I need are: id (guid), stream_id (guid), verion (int), data (jsonb), type (string),
// timestamp (datetimeoffset), tenant_id (string), mt_dotnet_type (string), 
// correlation_id (nullable string), causation_id (nullable string), is_archived (bool)
public class Event
{
  public Guid Id { get; set; }
  public Guid StreamId { get; set; }
  public int Version { get; set; }
  public string Data { get; set; }
  public string Type { get; set; }
  public DateTimeOffset Timestamp { get; set; }
  public string TenantId { get; set; }
  public string MtDotnetType { get; set; }
  public string CorrelationId { get; set; }
  public string CausationId { get; set; }
  public bool IsArchived { get; set; }
}
namespace Bloggy.Core.BusContracts.Events;

public record BusEvent
{
    public Guid EventUid { get; init; } = Guid.NewGuid();
    public ServiceName Publisher { get; init; }
    public DateTime EventTimestamp { get; init; } = DateTime.UtcNow;
}
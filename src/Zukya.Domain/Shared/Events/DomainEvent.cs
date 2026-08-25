namespace Zukya.Domain.Shared.Events;

public abstract class DomainEvent
{
    public DateTime OccuredOn { get; set; } = DateTime.UtcNow;
}

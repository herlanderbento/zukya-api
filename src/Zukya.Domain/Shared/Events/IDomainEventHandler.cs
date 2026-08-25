namespace Zukya.Domain.Shared.Events;

public interface IDomainEventHandler<in TDomainEvent>
    where TDomainEvent : DomainEvent
{
    Task HandleAsync(TDomainEvent domainEvent, CancellationToken cancellationToken);
}

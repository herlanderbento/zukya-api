namespace Zukya.Domain.Shared.Events;

public interface IDomainEventPublisher
{
    Task PublishAsync<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken)
        where TDomainEvent : DomainEvent;
}

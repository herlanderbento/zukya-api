using Zukya.Domain.Shared.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Zukya.Application.Common.Events;

public class DomainEventPublisher(IServiceProvider serviceProvider) : IDomainEventPublisher
{
    public async Task PublishAsync<TDomainEvent>(
        TDomainEvent domainEvent,
        CancellationToken cancellationToken
    )
        where TDomainEvent : DomainEvent
    {
        var handlers = serviceProvider.GetServices<IDomainEventHandler<TDomainEvent>>();
        if (handlers is null || !handlers.Any())
            return;
        foreach (var handler in handlers)
            await handler.HandleAsync(domainEvent, cancellationToken);
    }
}
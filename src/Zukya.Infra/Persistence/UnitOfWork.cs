using Microsoft.Extensions.Logging;
using Zukya.Application.Common.Interfaces;
using Zukya.Domain.Shared.Entities;
using Zukya.Domain.Shared.Events;

namespace Zukya.Infra.Persistence;

public class UnitOfWork(
    DatabaseContext context,
    IDomainEventPublisher publisher,
    ILogger<UnitOfWork> logger)
    : IUnitOfWork
{
    public async Task Commit(CancellationToken cancellationToken)
    {
        IEnumerable<AggregateRoot> aggregateRoots = context
            .ChangeTracker.Entries<AggregateRoot>()
            .Where(entry => entry.Entity.Events.Any())
            .Select(entry => entry.Entity);

        IEnumerable<AggregateRoot> enumerable = aggregateRoots.ToList();
        logger.LogInformation(
            "Commit: {AggregatesCount} aggregate roots with events.",
            enumerable.Count()
        );

        IEnumerable<DomainEvent> events = enumerable.SelectMany(aggregate => aggregate.Events);

        IEnumerable<DomainEvent> domainEvents = events.ToList();
        logger.LogInformation("Commit: {EventsCount} events raised.", domainEvents.Count());

        foreach (DomainEvent @event in domainEvents)
            await publisher.PublishAsync((dynamic)@event, cancellationToken);

        foreach (AggregateRoot aggregate in enumerable)
            aggregate.ClearEvents();

        await context.SaveChangesAsync(cancellationToken);
    }

    public Task Rollback(CancellationToken cancellationToken) => Task.CompletedTask;
}

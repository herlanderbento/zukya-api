using System.Collections.ObjectModel;
using Zukya.Domain.Shared.Events;

namespace Zukya.Domain.Shared.Entities;

public abstract class AggregateRoot : Entity
{
    private readonly List<DomainEvent> _events = new();
    public IReadOnlyCollection<DomainEvent> Events => new ReadOnlyCollection<DomainEvent>(_events);

    protected AggregateRoot()
        : base() { }

    public void RaiseEvent(DomainEvent @event) => _events.Add(@event);

    public void ClearEvents() => _events.Clear();
}

using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infrastructure.Handlers;

public class EventSourcingHandler(IEventStore eventStore) : IEventSourcingHandler<PostAggregate>
{
    private readonly IEventStore _eventStore = eventStore;

    public async Task<PostAggregate> GetByIdAsync(Guid aggregateId)
    {
        PostAggregate aggregate = new(); 
        var events = await _eventStore
            .FindByAggregateId(aggregateId)
            .ConfigureAwait(false);

        if (events == null || events.Count == 0) return aggregate;

        aggregate.ReplayEvents(events);
        aggregate.Version = events.Last().Version;

        return aggregate;
    }

    public async Task SaveAsync(AggregateRoot aggregate)
    {
        await _eventStore
            .SaveEventsAsync(
                aggregate.Id,
                aggregate.GetUncommittedChanges(), 
                aggregate.Version)
            .ConfigureAwait(false);

        aggregate.MarkChangesAsCommitted();
    }
}
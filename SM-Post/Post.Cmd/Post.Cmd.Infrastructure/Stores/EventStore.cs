namespace Post.Cmd.Infrastructure.Stores;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using MongoDB.Driver;
using Post.Cmd.Domain.Aggregates;

public class EventStore(
    IEventStoreRepository eventStoreRepository,
    IEventProducer eventProducer) : IEventStore
{    
    private readonly IEventStoreRepository _eventStoreRepository = eventStoreRepository;
    private readonly IEventProducer _eventProducer = eventProducer;
    

    public async Task<List<BaseEvent>> FindByAggregateId(Guid aggregateId)
    {
        var eventStream = await _eventStoreRepository
            .FindByAggregateId(aggregateId).ConfigureAwait(false);

        if (eventStream == null || !eventStream.Any())
            throw new AggregateNotFoundException($"Incorrect Post Id: {aggregateId}");

        return eventStream
            .OrderBy(e => e.Version)
            .Select(e => e.EventData)
            .ToList();
    }

    public async Task SaveEventsAsync(Guid aggregateId, 
        IEnumerable<BaseEvent> events, 
        int expectedVersion)
    {
        var eventStream = await _eventStoreRepository
            .FindByAggregateId(aggregateId)
            .ConfigureAwait(false);
        
        if (expectedVersion != -1 && eventStream.Any() && 
            eventStream.Last().Version != expectedVersion)
            throw new ConcurrencyException();
        
        var version = expectedVersion;

        foreach (var @event in events)
        {
            version++;
            @event.Version = version;

            var eventType = @event.GetType().Name;
            var eventModel = new EventModel
            {
                Timestamp = DateTime.UtcNow,
                AggregateIdentifier = aggregateId,
                AggregateType = nameof(PostAggregate),
                Version = version,
                EventType = eventType,
                EventData = @event
            };

            await _eventStoreRepository.SaveAsync(eventModel)
                .ConfigureAwait(false);

            var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
            await _eventProducer.ProduceAsync(topic, @event)
                .ConfigureAwait(false);
        }
    }
}
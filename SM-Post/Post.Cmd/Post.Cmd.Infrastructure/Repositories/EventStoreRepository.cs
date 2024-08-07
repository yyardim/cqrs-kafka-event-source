// Purpose: Contains the EventStoreRepository class which implements the IEventStoreRepository interface. This class is responsible for handling the event store repository. The interface is used to define the methods that the class must implement
using CQRS.Core.Domain;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Post.Cmd.Infrastructure.Config;

namespace Post.Cmd.Infrastructure.Repositories;
public class EventStoreRepository : IEventStoreRepository
{
    private readonly IMongoCollection<EventModel> _eventStoreCollection;
    public EventStoreRepository(IOptions<MongoDbConfig> config)
    {
        MongoClient mongoClient = new(config.Value.ConnectionString);
        IMongoDatabase mongoDbDatabase = mongoClient.GetDatabase(config.Value.Database);
       
        _eventStoreCollection = mongoDbDatabase
            .GetCollection<EventModel>(config.Value.Collection);
    }
    
    public async Task<IEnumerable<EventModel>> FindByAggregateId(Guid aggregateId)
    {
        return await _eventStoreCollection
            .Find(e => e.AggregateIdentifier == aggregateId)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task SaveAsync(EventModel @event)
    {
        await _eventStoreCollection
            .InsertOneAsync(@event)
            .ConfigureAwait(false);
    }
}
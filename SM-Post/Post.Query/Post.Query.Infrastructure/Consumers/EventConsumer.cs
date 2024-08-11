using System.Text.Json;
using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using Post.Query.Infrastructure.Converters;
using Post.Query.Infrastructure.Handlers;

namespace Post.Query.Infrastructure.Consumers;

public class EventConsumer(IOptions<ConsumerConfig> config, IEventHandler eventHandler) : IEventConsumer
{
    private readonly ConsumerConfig _config = config.Value;
    private readonly IEventHandler _eventHandler = eventHandler;

    public void Consume(string topic)
    {
        using var consumer = new ConsumerBuilder<string, string>(_config)
            .SetKeyDeserializer(Deserializers.Utf8)
            .SetValueDeserializer(Deserializers.Utf8)
            .Build();
        
        consumer.Subscribe(topic);

        while (true)
        {
            var consumeResult = consumer.Consume();

            if (consumeResult?.Message == null) continue;

            JsonSerializerOptions options = new() 
            { 
                Converters = { new EventJsonConverter() } 
            };
            
            var @event = JsonSerializer.Deserialize<BaseEvent>(consumeResult.Message.Value, options);
            var handlerMethod = _eventHandler.GetType().GetMethod("On", [@event.GetType()]);

            if (handlerMethod == null) 
            {
                throw new ArgumentNullException(nameof(handlerMethod), $"Handler method for {@event.GetType()} not found.");
            }

            handlerMethod.Invoke(_eventHandler, [@event]);
            consumer.Commit(consumeResult);
        }
    }
}
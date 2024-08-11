using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using CQRS.Core.Consumers;

namespace Post.Query.Infrastructure.Consumers;

public class ConsumerHostedService(
    ILogger<ConsumerHostedService> logger, 
    IServiceProvider serviceProvider) : IHostedService
{
    private readonly ILogger<ConsumerHostedService> _logger = logger;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Consumer hosted service started.");

        using var scope = _serviceProvider.CreateScope();

        var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();
        var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");

        Task.Run(() => eventConsumer.Consume(topic), cancellationToken);
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Consumer hosted service stopped.");
        
        return Task.CompletedTask;
    }
}
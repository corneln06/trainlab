using Microsoft.Extensions.DependencyInjection;

namespace TrainLab.SharedKernel.Events;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent;
}

public sealed class InProcessEventPublisher(IServiceProvider serviceProvider)
    : IEventPublisher
{
    public async Task PublishAsync<TEvent>(TEvent domainEvent,
        CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent
    {
        foreach (var handler in serviceProvider.GetServices<IEventHandler<TEvent>>())
        {
            await handler.HandleAsync(domainEvent, cancellationToken);
        }
    }
}
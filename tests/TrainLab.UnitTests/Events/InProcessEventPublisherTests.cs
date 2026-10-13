using Microsoft.Extensions.DependencyInjection;

using Shouldly;

using TrainLab.SharedKernel.Events;

namespace TrainLab.UnitTests.Events;

public class InProcessEventPublisherTests
{
    private sealed record TestEvent(string Name) : IDomainEvent;
    private sealed class CallLog
    {
        public List<string> Calls { get; } = [];
    }

    private sealed class FirstHandler(CallLog log) : IEventHandler<TestEvent>
    {
        public Task HandleAsync(TestEvent e, CancellationToken ct)
        {
            log.Calls.Add($"first:{e.Name}");
            return Task.CompletedTask;
        }
    }

    private sealed class SecondHandler(CallLog log) : IEventHandler<TestEvent>
    {
        public Task HandleAsync(TestEvent e, CancellationToken ct)
        {
            log.Calls.Add($"second:{e.Name}");
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task Publishing_an_event_calls_every_registered_handler()
    {
        var services = new ServiceCollection()
            .AddSingleton<CallLog>()
            .AddScoped<IEventHandler<TestEvent>, FirstHandler>()
            .AddScoped<IEventHandler<TestEvent>, SecondHandler>()
            .AddScoped<IEventPublisher, InProcessEventPublisher>()
            .BuildServiceProvider();

        await using var scope = services.CreateAsyncScope();
        var publisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();

        await publisher.PublishAsync(new TestEvent("ride"));

        scope.ServiceProvider.GetRequiredService<CallLog>().Calls
            .ShouldBe(["first:ride", "second:ride"]);
    }
}
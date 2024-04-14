using DotNetCore.CAP;
using MediaEncoder.Domain.Events;
using MediatR;

namespace MediaEncoder.WebAPI.EventHandlers;
class EncodingItemStartedEventHandler : INotificationHandler<EncodingItemStartedEvent>
{
    private readonly ICapPublisher capPublisher;

    public EncodingItemStartedEventHandler(ICapPublisher capPublisher)
    {
        this.capPublisher = capPublisher;
    }
    public Task Handle(EncodingItemStartedEvent notification, CancellationToken cancellationToken)
    {
        capPublisher.Publish("MediaEncoding.Started", notification);
        return Task.CompletedTask;
    }
}
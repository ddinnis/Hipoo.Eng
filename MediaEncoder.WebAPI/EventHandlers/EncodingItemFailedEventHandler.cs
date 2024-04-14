using DotNetCore.CAP;
using MediaEncoder.Domain.Events;
using MediatR;

namespace MediaEncoder.WebAPI.EventHandlers;
class EncodingItemFailedEventHandler : INotificationHandler<EncodingItemFailedEvent>
{
    private readonly  ICapPublisher capPublisher;

    public EncodingItemFailedEventHandler(ICapPublisher capPublisher)
    {
        this.capPublisher = capPublisher;
    }
    public Task Handle(EncodingItemFailedEvent notification, CancellationToken cancellationToken)
    {
        capPublisher.Publish("MediaEncoding.Failed", notification);
        return Task.CompletedTask;
    }
}
using DotNetCore.CAP;
using MediaEncoder.Domain.Events;
using MediatR;
using DotNetCore.CAP;
using MediatR;

namespace MediaEncoder.WebAPI.EventHandlers;
class EncodingItemCreatedEventHandler : INotificationHandler<EncodingItemCreatedEvent>
{
    private readonly ICapPublisher capPublisher;

    public EncodingItemCreatedEventHandler(ICapPublisher capPublisher)
    {
        this.capPublisher = capPublisher;
    }

    public Task Handle(EncodingItemCreatedEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
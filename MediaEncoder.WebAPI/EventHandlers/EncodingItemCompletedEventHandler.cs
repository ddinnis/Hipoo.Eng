using MediaEncoder.Domain.Events;
using DotNetCore.CAP;
using MediatR;

namespace MediaEncoder.WebAPI.EventHandlers;
class EncodingItemCompletedEventHandler : INotificationHandler<EncodingItemCompletedEvent>
{
    private readonly ICapPublisher capPublisher;

    public EncodingItemCompletedEventHandler(ICapPublisher capPublisher)
    {
        this.capPublisher = capPublisher;
    }

    public Task Handle(EncodingItemCompletedEvent notification, CancellationToken cancellationToken)
    {
        //把转码任务状态变化的领域事件，转换为集成事件发出
        capPublisher.Publish("MediaEncoding.Completed", notification);
        return Task.CompletedTask;
    }
}
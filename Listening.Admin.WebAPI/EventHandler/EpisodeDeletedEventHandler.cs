namespace Listening.Admin.WebAPI.EventHandlers;
public class EpisodeDeletedEventHandler : INotificationHandler<EpisodeDeletedEvent>
{
    private readonly ICapPublisher _capBus;
    public EpisodeDeletedEventHandler(ICapPublisher capBus)
    {
        _capBus = capBus;
    }

    public Task Handle(EpisodeDeletedEvent notification, CancellationToken cancellationToken)
    {
        var id = notification.Id;
        _capBus.Publish("ListeningEpisode.Deleted", new { Id = id });
        return Task.CompletedTask;
    }
}

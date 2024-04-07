using DotNetCore.CAP;
using Listening.Domain.Events;
using MediatR;

namespace Listening.Admin.WebAPI.EventHandler
{
    public class EpisodeCreatedEventHandler :INotificationHandler<EpisodeCreatedEvent>
    {
        private readonly ICapPublisher _capBus;
        public EpisodeCreatedEventHandler(ICapPublisher capBus)
        {
            _capBus = capBus;
        }
        public Task Handle(EpisodeCreatedEvent notification, CancellationToken cancellationToken)
        {
            var episode = notification.Value;
            var sentences = episode.ParseSubtitle();
            _capBus.Publish("ListeningEpisode.Created", new { Id = episode.Id, episode.Name, Sentences = sentences, episode.AlbumId, episode.Subtitle, episode.SubtitleType });
            return Task.CompletedTask;
        }
    }
}

using Common.EventBus;
using Listening.Admin.WebAPI.Events;
using Listening.Admin.WebAPI.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Listening.Admin.WebAPI.EventHandler
{
    
    public class MediaEncodingStatusChangeIntegrationHandler
    {
        private readonly ListeningDbContext dbContext;
        private readonly IListeningRepository repository;
        private readonly EncodingEpisodeHelper encHelper;
        private readonly IHubContext<EpisodeEncodingStatusHub> hubContext;

        public MediaEncodingStatusChangeIntegrationHandler(ListeningDbContext dbContext,
        EncodingEpisodeHelper encHelper,
        IHubContext<EpisodeEncodingStatusHub> hubContext, IListeningRepository repository)
        {
            this.dbContext = dbContext;
            this.encHelper = encHelper;
            this.hubContext = hubContext;
            this.repository = repository;
        }

        [CapSubscribe("MediaEncoding.Started")]
        public async Task HandleSubscriber(EpisodeStartedEvent EventData)
        {
            await encHelper.UpdateEpisodeStatusAsync(EventData.Id, "Started");
            await hubContext.Clients.All.SendAsync("OnMediaEncodingStarted", EventData.Id);
        }

        [CapSubscribe("MediaEncoding.Failed")]
        public async Task HandleSubscriber(EpisodeFailedEvent EventData)
        {
            await encHelper.UpdateEpisodeStatusAsync(EventData.Id, "Failed");
            await hubContext.Clients.All.SendAsync("OnMediaEncodingFailed", EventData.Id);
        }

        [CapSubscribe("MediaEncoding.Duplicated")]
        public async Task HandleSubscriber(EpisodeDuplicatedEvent EventData)
        {
            await encHelper.UpdateEpisodeStatusAsync(EventData.Id, "Completed");
            await hubContext.Clients.All.SendAsync("OnMediaEncodingCompleted", EventData.Id);
        }

        [CapSubscribe("MediaEncoding.Completed")]
        public async Task HandleSubscriber(EpisodeCompletedEvent EventData)
        {
            await encHelper.UpdateEpisodeStatusAsync(EventData.Id, "Completed");
            // todo...
        }
    }
}

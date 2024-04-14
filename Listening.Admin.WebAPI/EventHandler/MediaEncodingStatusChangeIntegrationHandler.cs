using Common.EventBus;
using Listening.Admin.WebAPI.Hubs;
using MediaEncoder.Domain.Events;
using Microsoft.AspNetCore.SignalR;

namespace Listening.Admin.WebAPI.EventHandler
{
    
    public class MediaEncodingStatusChangeIntegrationHandler: ICapSubscribe
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
        public async Task HandleSubscriber(EncodingItemStartedEvent EventData)
        {
            await encHelper.UpdateEpisodeStatusAsync(EventData.Id, "Started");
            await hubContext.Clients.All.SendAsync("OnMediaEncodingStarted", EventData.Id);
        }

        [CapSubscribe("MediaEncoding.Failed")]
        public async Task HandleSubscriber(EncodingItemFailedEvent EventData)
        {
            await encHelper.UpdateEpisodeStatusAsync(EventData.Id, "Failed");
            await hubContext.Clients.All.SendAsync("OnMediaEncodingFailed", EventData.Id);
        }

        [CapSubscribe("MediaEncoding.Duplicated")]
        public async Task HandleSubscriber(EncodingItemCompletedEvent EventData)
        {
            await encHelper.UpdateEpisodeStatusAsync(EventData.Id, "Completed");
            await hubContext.Clients.All.SendAsync("OnMediaEncodingCompleted", EventData.Id);
        }

        [CapSubscribe("MediaEncoding.Completed")]
        public async Task HandleCompletedSubscriber(EncodingItemCompletedEvent EventData)
        {
            Guid id = EventData.Id;
            await encHelper.UpdateEpisodeStatusAsync(id, "Completed");
            Uri outputUrl = EventData.OutputUrl;
            var encItem = await encHelper.GetEncodingEpisodeAsync(id);

            Guid albumId = encItem.AlbumId;
            int maxSeq = await repository.GetMaxSeqOfEpisodesAsync(albumId);
            
            var builder = new Episode.Builder();
            builder.Id(id).SequenceNumber(maxSeq + 1).Name(encItem.Name)
                .AlbumId(albumId).AudioUrl(outputUrl)
                .DurationInSecond(encItem.DurationInSecond)
                .SubtitleType(encItem.SubtitleType).Subtitle(encItem.Subtitle);
            var episdoe = builder.Build();
            dbContext.Add(episdoe);
            await dbContext.SaveChangesAsync();
            // 通知前端刷新
            await hubContext.Clients.All.SendAsync("OnMediaEncodingCompleted", id);
        }
    }
}

using Common.Domain.Models;
using DotNetCore.CAP;
using SearchService.Domain;

namespace SearchService.WebAPI.EventHandlers
{
    public record ListeningEpisodeUpsertEvent
    (
       Guid Id, MultilingualString Name, IEnumerable<dynamic> Sentences, Guid AlbumId, string strSubtitle, string SubtitleType
    );

  
    [CapSubscribe("ListeningEpisode.Created")]
    [CapSubscribe("ListeningEpisode.Updated")]
    public class ListeningEpisodeUpsertEventHandler:ICapSubscribe
    {
        private readonly ISearchRepository repository;

        public ListeningEpisodeUpsertEventHandler(ISearchRepository repository)
        {
            this.repository = repository;
        }

        public Task HandleSubscriber(ListeningEpisodeUpsertEvent EventData)
        {
            Guid id = EventData.Id;
            string cnName = EventData.Name.Chinese;
            string engName = EventData.Name.English;
            Guid albumId = EventData.AlbumId;
            List<string> sentences = new List<string>();

            foreach (var sentence in EventData.Sentences)
            {
                sentences.Add(sentence.Value);
            }
            string plainSentences = string.Join("\r\n", sentences);
            Episode episode = new Episode(id, cnName, engName, plainSentences, albumId);
            return repository.UpdateAsync(episode);
        }
    }
}

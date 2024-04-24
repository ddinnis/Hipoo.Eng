using DotNetCore.CAP;
using SearchService.Domain;

namespace SearchService.WebAPI.EventHandlers
{
    public record ListeningEpisodeDeletedEvent(Guid Id );


    [CapSubscribe("ListeningEpisode.Deleted")]
    [CapSubscribe("ListeningEpisode.Hidden")]//被隐藏也看作删除
    public class ListeningEpisodeDeletedEventHandler:ICapSubscribe
    {
        private readonly ISearchRepository repository;

        public ListeningEpisodeDeletedEventHandler(ISearchRepository repository)
        {
            this.repository = repository;
        }

        public Task HandleSubscriber(ListeningEpisodeDeletedEvent EventData) 
        {
            Guid guid = EventData.Id;
            return repository.DeleteAsync(guid);
        }

    }
}

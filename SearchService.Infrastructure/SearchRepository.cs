using Common.Common;
using Nest;
using SearchService.Domain;

namespace SearchService.Infrastructure
{
    public class SearchRepository : ISearchRepository
    {
        private readonly IElasticClient elasticClient;

        public SearchRepository(IElasticClient elasticClient)
        {
            this.elasticClient = elasticClient;
        }

        public Task DeleteAsync(Guid episodeId)
        {
            elasticClient.DeleteByQuery<Episode>(q => q
            .Index("episodes")
            .Query(rq => rq
                .Term(f => f.Id, "elasticsearch.pm")
                )
            );
            return elasticClient.DeleteAsync(new DeleteRequest("episodes",episodeId));
        }

        public async Task<SearchEpisodesResponse> SearchEpisodes(string keyWord, int PageIndex, int PageSize)
        {

            int from = PageSize * (PageIndex - 1);
            string kw = keyWord;
            Func<QueryContainerDescriptor<Episode>, QueryContainer> query = (q) =>
                          q.Match(mq => mq.Field(f => f.CnName).Query(kw))
                          || q.Match(mq => mq.Field(f => f.EngName).Query(kw))
                          || q.Match(mq => mq.Field(f => f.PlainSubtitle).Query(kw));
            Func<HighlightDescriptor<Episode>, IHighlight> highlightSelector = h => h
                .Fields(fs => fs.Field(f => f.PlainSubtitle));
            var result = await this.elasticClient.SearchAsync<Episode>(s => s.Index("episodes").From(from)
                .Size(PageSize).Query(query).Highlight(highlightSelector));
            if (!result.IsValid)
            {
                throw result.OriginalException;
            }
            List<Episode> episodes = new List<Episode>();
            foreach (var hit in result.Hits)
            {
                string highlightedSubtitle;
                //如果没有预览内容，则显示前50个字
                if (hit.Highlight.ContainsKey("plainSubtitle"))
                {
                    highlightedSubtitle = string.Join("\r\n", hit.Highlight["plainSubtitle"]);
                }
                else
                {
                    highlightedSubtitle = hit.Source.PlainSubtitle.Cut(50);
                }
                var episode = hit.Source with { PlainSubtitle = highlightedSubtitle };
                episodes.Add(episode);
            }
            return new SearchEpisodesResponse(episodes, result.Total);
        }

        public async Task UpdateAsync(Episode episode)
        {
            var response = await elasticClient.IndexAsync(episode, idx => idx.Index("episodes").Id(episode.Id));
            if (!response.IsValid)
            {
                throw new ApplicationException(response.DebugInformation);
            }
        }
    }
}

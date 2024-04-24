
namespace SearchService.Domain
{
    public record SearchEpisodesResponse(IEnumerable<Episode> episodes,long TotalCount);
}


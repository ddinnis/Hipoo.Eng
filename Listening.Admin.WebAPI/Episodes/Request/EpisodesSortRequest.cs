
using FluentValidation;

namespace Listening.Admin.WebAPI.Episodes;
public class EpisodesSortRequest
{
    public Guid[] SortedEpisodeIds { get; set; }
}

public class EpisodesSortRequestValidator : AbstractValidator<EpisodesSortRequest>
{
    public EpisodesSortRequestValidator()
    {
        RuleFor(r => r.SortedEpisodeIds).NotNull().NotEmpty()
            .Must(x => x == null | !x.Contains(Guid.Empty))
            // 集合中没有重复元素
            .Must(x => x == null | x.Distinct().Count() == x.Count());
    }
}

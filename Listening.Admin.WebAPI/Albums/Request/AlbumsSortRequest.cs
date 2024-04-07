namespace Listening.Admin.WebAPI.Albums;
public class AlbumsSortRequest
{
    public Guid[] SortedAlbumIds { get; set; }
}

public class AlbumsSortRequestValidator : AbstractValidator<AlbumsSortRequest>
{
    public AlbumsSortRequestValidator()
    {
        RuleFor(r => r.SortedAlbumIds).NotNull().NotEmpty().Must(p => p == null || !p.Contains(Guid.Empty))
            .Must(p => p == null || p.Distinct().Count() == p.Count());
    }
}

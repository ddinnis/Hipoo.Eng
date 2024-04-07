namespace Listening.Admin.WebAPI.Categories;
public class CategoriesSortRequest
{
    /// <summary>
    /// 排序后的类别Id
    /// </summary>
    public Guid[] SortedCategoryIds { get; set; }
}

public class CategoriesSortRequestValidator : AbstractValidator<CategoriesSortRequest>
{
    public CategoriesSortRequestValidator()
    {
        RuleFor(r => r.SortedCategoryIds).NotNull().NotEmpty().Must(p => p == null || !p.Contains(Guid.Empty))
            .Must(p => p == null || p.Distinct().Count() == p.Count());
    }
}
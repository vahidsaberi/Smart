namespace Base.Application.Common.Models;

public class BaseFilter
{
    public Search? AdvancedSearch { get; set; }
    public string? Keyword { get; set; }
    public Filter? AdvancedFilter { get; set; }
}
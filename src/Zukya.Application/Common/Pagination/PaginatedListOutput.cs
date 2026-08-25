namespace Zukya.Application.Common.Pagination;

public abstract class PaginatedListOutput<TOutputItem>(
    int page,
    int perPage,
    int total,
    IReadOnlyList<TOutputItem> items)
{
    public int Page { get; set; } = page;
    public int PerPage { get; set; } = perPage;
    public int LastPage { get; set; } = perPage > 0 ? (int)Math.Ceiling((double)total / perPage) : 0;
    public int Total { get; set; } = total;

    public IReadOnlyList<TOutputItem> Items { get; set; } = items;
}
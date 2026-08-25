namespace Zukya.Domain.Shared.Repositories;


public class SearchInput<TSearchInput>(
    int page,
    int perPage,
    TSearchInput search,
    string orderBy,
    SearchOrder order)
{
    public int Page { get; set; } = page;
    public int PerPage { get; set; } = perPage;
    public TSearchInput Search { get; set; } = search;
    public string OrderBy { get; set; } = orderBy;
    public SearchOrder Order { get; set; } = order;
}

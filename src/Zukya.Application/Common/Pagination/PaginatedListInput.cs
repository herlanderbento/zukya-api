

using Zukya.Domain.Shared.Repositories;

namespace Zukya.Application.Common.Pagination;

public abstract class PaginatedListInput<TSearch>(int page, int perPage, TSearch search, string sort, SearchOrder dir)
{
    public int Page { get; set; } = page;
    public int PerPage { get; set; } = perPage;
    public TSearch Search { get; set; } = search;
    public string Sort { get; set; } = sort;
    public SearchOrder Dir { get; set; } = dir;

    public SearchInput<TSearch> ToSearchInput() => new(Page, PerPage, Search, Sort, Dir);
}

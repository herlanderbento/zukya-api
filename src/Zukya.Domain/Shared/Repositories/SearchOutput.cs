namespace Zukya.Domain.Shared.Repositories;

public class SearchOutput<TSearchOutput>(int currentPage, int perPage, int total, IReadOnlyList<TSearchOutput> items)
{
    public int CurrentPage { get; set; } = currentPage;
    public int PerPage { get; set; } = perPage;
    public int Total { get; set; } = total;
    public IReadOnlyList<TSearchOutput> Items { get; set; } = items;
}
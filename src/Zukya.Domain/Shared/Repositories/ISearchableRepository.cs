namespace Zukya.Domain.Shared.Repositories;

public interface ISearchableRepository<TAggregate, TSearch>
{
    Task<SearchOutput<TAggregate>> Search(
        SearchInput<TSearch> input,
        CancellationToken cancellationToken
    );
}
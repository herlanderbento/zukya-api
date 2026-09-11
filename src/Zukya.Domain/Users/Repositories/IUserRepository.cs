using Zukya.Domain.Shared.Repositories;
using Zukya.Domain.Users.Entities;

namespace Zukya.Domain.Users.Repositories;

public interface IUserRepository : IRepository<User>, ISearchableRepository<User, string>
{
    Task<User> GetByEmail(string email, CancellationToken cancellationToken = default);
    Task<User> GetByPhone(string phone, CancellationToken cancellationToken = default);

    Task<User> GetByEmailOrPhone(
        string identifier,
        CancellationToken cancellationToken = default
    );

    Task<User> GetByTaxId(
        string taxId,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<User>> GetListByIds(
        List<Guid> ids,
        CancellationToken cancellationToken
    );
}

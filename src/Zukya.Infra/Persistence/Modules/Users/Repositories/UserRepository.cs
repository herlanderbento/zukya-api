using Microsoft.EntityFrameworkCore;
using Zukya.Domain.Shared.Repositories;
using Zukya.Domain.Users.Entities;
using Zukya.Domain.Users.Repositories;

namespace Zukya.Infra.Persistence.Modules.Users.Repositories;

public class UserRepository(DatabaseContext context) : IUserRepository
{
    private DbSet<User> Users => context.Set<User>();

    public async Task Insert(User aggregate, CancellationToken cancellationToken) =>
        await Users.AddAsync(aggregate, cancellationToken);

    public async Task<User> GetById(Guid id, CancellationToken cancellationToken)
    {
        User? model = await Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return model!;
    }

    public async Task<User> GetByEmail(string email, CancellationToken cancellationToken)
    {
        User? model = await Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

        return model!;
    }

    public async Task<User> GetByEmailOrPhone(string identifier, CancellationToken cancellationToken = default)
    {
        User? model = await Users
            .FirstOrDefaultAsync(x => x.Email == identifier || x.Phone == identifier, cancellationToken);

        return model!;
    }

    public async Task<User> GetByTaxId(
        string taxId,
        CancellationToken cancellationToken
    )
    {
        User? model = await Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TaxId == taxId, cancellationToken);

        return model!;
    }

    public async Task<User> GetByPhone(string phone, CancellationToken cancellationToken)
    {
        User? model = await Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Phone == phone, cancellationToken);

        return model!;
    }

    public async Task<IReadOnlyList<User>> GetListByIds(
        List<Guid> ids,
        CancellationToken cancellationToken
    )
    {
        List<User> models = await Users
            .AsNoTracking()
            .Where(user => ids.Contains(user.Id))
            .ToListAsync(cancellationToken);

        return models;
    }

    public async Task<SearchOutput<User>> Search(
        SearchInput<string> input,
        CancellationToken cancellationToken
    )
    {
        var toSkip = (input.Page - 1) * input.PerPage;
        IQueryable<User> query = Users.AsNoTracking();

        
        query = AddOrderToQuery(query, input.OrderBy, input.Order);
        
        if (!string.IsNullOrWhiteSpace(input.Search))
        {
            var searchTerm = $"%{input.Search}%";

            query = query.Where(x =>
                x.Name.Contains(searchTerm) ||
                x.Email!.Contains(searchTerm) ||
                x.Phone!.Contains(searchTerm) ||
                (x.TaxId != null && EF.Functions.Like(x.TaxId, searchTerm))
            );
        }

        var total = await query.CountAsync(cancellationToken);
        List<User> items = await query
            .Skip(toSkip)
            .Take(input.PerPage)
            .ToListAsync(cancellationToken);

        return new SearchOutput<User>(input.Page, input.PerPage, total, items);
    }

    public Task Update(User aggregate, CancellationToken _) =>
        Task.FromResult(Users.Update(aggregate));

    public Task Delete(User aggregate, CancellationToken _) =>
        Task.FromResult(Users.Remove(aggregate));

    private static IQueryable<User> AddOrderToQuery(
        IQueryable<User> query,
        string orderProperty,
        SearchOrder order
    )
    {
        return (orderProperty.ToLower(), order) switch
        {
            ("name", SearchOrder.Asc) => query.OrderBy(x => x.Name),
            ("name", SearchOrder.Desc) => query.OrderByDescending(x => x.Name),
            ("createdAt", SearchOrder.Asc) => query.OrderBy(x => x.CreatedAt),
            ("createdAt", SearchOrder.Desc) => query.OrderByDescending(x => x.CreatedAt),
            _ => query.OrderBy(x => x.Name)
        };
    }
}

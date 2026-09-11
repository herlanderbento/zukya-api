using Microsoft.EntityFrameworkCore;
using Zukya.Domain.Users.Entities;
using Zukya.Domain.Users.Repositories;

namespace Zukya.Infra.Persistence.Modules.Users.Repositories;

public class VerificationCodeRepository(DatabaseContext context) : IVerificationCodeRepository
{
    private DbSet<VerificationCode> VerificationCode => context.Set<VerificationCode>();

    public async Task Insert(VerificationCode aggregate, CancellationToken cancellationToken) =>
        await VerificationCode.AddAsync(aggregate, cancellationToken);

    public async Task<VerificationCode> GetById(Guid id, CancellationToken cancellationToken)
    {
        VerificationCode? model = await VerificationCode.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        return model!;
    }

    public async Task<VerificationCode> GetByUserId(Guid userId, CancellationToken cancellationToken = default)
    {
        VerificationCode? model = await VerificationCode
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        return model!;
    }

    public async Task<VerificationCode> GetByCode(string code, CancellationToken cancellationToken = default)
    {
        VerificationCode? model = await VerificationCode
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == code, cancellationToken);

        return model!;
    }

    public Task Update(VerificationCode aggregate, CancellationToken cancellationToken) =>
        Task.FromResult(VerificationCode.Update(aggregate));

    public Task Delete(VerificationCode aggregate, CancellationToken cancellationToken) =>
        Task.FromResult(VerificationCode.Remove(aggregate));
}

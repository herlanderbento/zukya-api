using Zukya.Domain.Shared.Repositories;
using Zukya.Domain.Users.Entities;

namespace Zukya.Domain.Users.Repositories;

public interface IVerificationCodeRepository : IRepository<VerificationCode>
{
    Task<VerificationCode> GetByUserId(Guid userId, CancellationToken cancellationToken = default);
    Task<VerificationCode> GetByCode(string code, CancellationToken cancellationToken = default);
}

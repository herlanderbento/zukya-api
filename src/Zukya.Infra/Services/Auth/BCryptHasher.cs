using Zukya.Application.Common.Interfaces;

namespace Zukya.Infra.Services.Auth;

public class BCryptHasher : ICryptography
{
    private const int WorkFactor = 10;

    public Task<string> HashPassword(
        string password,
        CancellationToken cancellationToken) =>
        Task.FromResult(BCrypt.Net.BCrypt.HashPassword(password, WorkFactor));

    public Task<bool> Verify(
        string password,
        string passwordHash,
        CancellationToken cancellationToken
    ) =>
        Task.FromResult(BCrypt.Net.BCrypt.Verify(password, passwordHash));
}

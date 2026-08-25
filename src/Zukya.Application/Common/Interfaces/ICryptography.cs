namespace Zukya.Application.Common.Interfaces;

public interface ICryptography
{
    public Task<string> HashPassword(string password, CancellationToken cancellationToken);
    public Task<bool> Verify(
        string password,
        string passwordHash,
        CancellationToken cancellationToken
    );
}
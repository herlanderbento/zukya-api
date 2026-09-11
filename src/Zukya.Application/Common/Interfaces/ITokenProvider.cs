using Zukya.Domain.Users.Entities;

namespace Zukya.Application.Common.Interfaces;

public class TokenResponse
{
    public string AccessToken { get; set; } = null!;
    public int ExpiresIn { get; set; }
}

public interface ITokenProvider
{
    Task<TokenResponse> GenerateToken(User payload);
    Task<bool> ValidateToken(string token);
}

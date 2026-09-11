using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Zukya.Application.Common.Interfaces;
using Zukya.Domain.Users.Entities;

namespace Zukya.Infra.Services.Auth;

public class JwtTokenService : ITokenProvider
{
    private readonly RSA rsa;
    private const int AccessTokenExpirationHours = 4; // 4 horas de sessão para o e-commerce
    private readonly string publicKey;

    public JwtTokenService(IConfiguration configuration)
    {
        if (configuration == null)
            throw new ArgumentNullException(
                nameof(configuration),
                "Configuration was not injected correctly"
            );

        var privateKeyBase64 = configuration["Jwt:PrivateKey"];
        var publicKeyBase64 = configuration["Jwt:PublicKey"];

        if (string.IsNullOrEmpty(privateKeyBase64))
            throw new ArgumentNullException(
                "Jwt:PrivateKey",
                "JWT Private Key is missing in configuration."
            );

        if (string.IsNullOrEmpty(publicKeyBase64))
            throw new ArgumentNullException(
                "Jwt:PublicKey",
                "JWT Public Key is missing in configuration."
            );

        var cleanPrivateKey = CleanKey(privateKeyBase64);
        publicKey = CleanKey(publicKeyBase64);

        var privateKeyPreview =
            cleanPrivateKey.Length > 20
                ? cleanPrivateKey.Substring(0, 20) + "..."
                : cleanPrivateKey;
        Console.WriteLine($"Loaded Private Key: {privateKeyPreview}");

        rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(Convert.FromBase64String(cleanPrivateKey), out _);
    }

    public Task<TokenResponse> GenerateToken(User payload)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var credentials = new SigningCredentials(
            new RsaSecurityKey(rsa),
            SecurityAlgorithms.RsaSha256
        );

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, payload.Id.ToString()!),
            new(JwtRegisteredClaimNames.Name, payload.Name!),
            new(JwtRegisteredClaimNames.Email, payload.Email ?? string.Empty),
            new("role", payload.AccessLevel.ToString()!)
        };

        var accessToken = new JwtSecurityToken(
            "Zukya",
            "Zukya",
            claims,
            expires: DateTime.UtcNow.AddHours(AccessTokenExpirationHours),
            signingCredentials: credentials
        );

        var accessTokenString = tokenHandler.WriteToken(accessToken);

        var response = new TokenResponse
        {
            AccessToken = accessTokenString, ExpiresIn = AccessTokenExpirationHours * 60 * 60
        };

        return Task.FromResult(response);
    }

    public Task<bool> ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            using var rsaPublic = RSA.Create();
            rsaPublic.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);

            tokenHandler.ValidateToken(
                token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new RsaSecurityKey(rsaPublic),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = "Zukya",
                    ValidAudience = "Zukya",
                    ClockSkew = TimeSpan.Zero
                },
                out _
            );

            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    private static string CleanKey(string key)
    {
        return key
            .Replace("-----BEGIN PRIVATE KEY-----", "")
            .Replace("-----END PRIVATE KEY-----", "")
            .Replace("-----BEGIN PUBLIC KEY-----", "")
            .Replace("-----END PUBLIC KEY-----", "")
            .Replace("-----BEGIN RSA PRIVATE KEY-----", "")
            .Replace("-----END RSA PRIVATE KEY-----", "")
            .Replace("-----BEGIN RSA PUBLIC KEY-----", "")
            .Replace("-----END RSA PUBLIC KEY-----", "")
            .Replace("\n", "")
            .Replace("\r", "")
            .Replace(" ", "")
            .Replace("\t", "")
            .Trim();
    }
}

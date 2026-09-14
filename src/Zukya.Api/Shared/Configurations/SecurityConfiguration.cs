using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Zukya.Api.Shared.Configurations;

public static class AccessLevels
{
    public const int Client = 0;
    public const int Seller = 1;
    public const int Admin = 5;
}

public static class SecurityConfiguration
{
    public static IServiceCollection AddSecurity(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddJwtAuthentication(configuration);

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireAssertion(context =>
                {
                    var roleClaim = context.User.FindFirst("role")?.Value;
                    return int.TryParse(roleClaim, out var level) && level >= AccessLevels.Admin;
                }));

            options.AddPolicy("AtLeastSeller", policy =>
                policy.RequireAssertion(context =>
                {
                    var roleClaim = context.User.FindFirst("role")?.Value;
                    return int.TryParse(roleClaim, out var level) && level >= AccessLevels.Seller;
                }));

            options.AddPolicy("ClientOrAbove", policy =>
                policy.RequireAssertion(context =>
                {
                    var roleClaim = context.User.FindFirst("role")?.Value;
                    return int.TryParse(roleClaim, out var level) && level >= AccessLevels.Client;
                }));

            options.AddPolicy("Authenticated", policy => policy.RequireAuthenticatedUser());
        });

        return services;
    }

    private static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        IConfigurationSection jwtSection = configuration.GetRequiredSection("Jwt");
        var publicKeyBase64 =
            jwtSection["PublicKey"]
            ?? throw new ArgumentNullException("Jwt:PublicKey is missing in configuration.");

        var issuer = (jwtSection["Issuer"] ?? "Zukya").Trim();
        var audience = (jwtSection["Audience"] ?? "Zukya").Trim();

        if (issuer.Equals("zukya", StringComparison.OrdinalIgnoreCase)) issuer = "Zukya";
        if (audience.Equals("zukya", StringComparison.OrdinalIgnoreCase)) audience = "Zukya";

        var rsa = RSA.Create();
        var publicKeyBytes = Convert.FromBase64String(publicKeyBase64);

        try
        {
            rsa.ImportRSAPublicKey(publicKeyBytes, out _);
        }
        catch (Exception ex)
        {
            try
            {
                rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);
            }
            catch (Exception ex2)
            {
                throw new InvalidOperationException(
                    $"Failed to import RSA public key. PKCS#1 error: {ex.Message}. SPKI error: {ex2.Message}",
                    ex2
                );
            }
        }

        var key = new RsaSecurityKey(rsa);

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    ValidAudiences = new[] { audience },
                    IssuerSigningKey = key,
                    RoleClaimType = "role",
                    NameClaimType = JwtRegisteredClaimNames.Sub,
                    ClockSkew = TimeSpan.Zero
                };
            });

        return services;
    }
}

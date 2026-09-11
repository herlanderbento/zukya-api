using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Zukya.Api.Shared.Configurations;

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
            options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
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

        // If environment variable is lowercase, convert to match token
        if (issuer.Equals("zukya", StringComparison.OrdinalIgnoreCase)) issuer = "Zukya";
        if (audience.Equals("zukya", StringComparison.OrdinalIgnoreCase)) audience = "Zukya";

        Console.WriteLine(
            $"JWT Configuration - Issuer: '{issuer}' (length: {issuer.Length}), Audience: '{audience}' (length: {audience.Length})"
        );

        var rsa = RSA.Create();
        var publicKeyBytes = Convert.FromBase64String(publicKeyBase64);

        // Try ImportRSAPublicKey first (PKCS#1 format)
        try
        {
            rsa.ImportRSAPublicKey(publicKeyBytes, out _);
        }
        catch (Exception ex)
        {
            // If that fails, try ImportSubjectPublicKeyInfo (SPKI format)
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
                var tokenValidationParams = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience, // Use ValidAudience for string audience
                    ValidAudiences = new[] { audience }, // Also set as array for compatibility
                    IssuerSigningKey = key,
                    RoleClaimType = "role", // Match the claim name used in JwtTokenService
                    NameClaimType = JwtRegisteredClaimNames.Sub,
                    ClockSkew = TimeSpan.Zero
                };

                Console.WriteLine(
                    $"TokenValidationParameters - ValidIssuer: '{tokenValidationParams.ValidIssuer}', ValidAudience: '{tokenValidationParams.ValidAudience}', ValidAudiences: [{string.Join(", ", tokenValidationParams.ValidAudiences ?? Array.Empty<string>())}]"
                );

                options.TokenValidationParameters = tokenValidationParams;

                // Configure events to help debug authentication issues
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                        Console.WriteLine($"Exception type: {context.Exception.GetType().Name}");
                        if (context.Exception.InnerException != null)
                            Console.WriteLine(
                                $"Inner exception: {context.Exception.InnerException.Message}"
                            );

                        // Try to extract token info for debugging
                        HttpRequest request = context.Request;
                        var authHeader = request.Headers.Authorization.FirstOrDefault();
                        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                            return Task.CompletedTask;
                        var token = authHeader[7..];
                        try
                        {
                            var handler =
                                new JwtSecurityTokenHandler();
                            JwtSecurityToken? jsonToken = handler.ReadJwtToken(token);
                            Console.WriteLine(
                                $"Token Audience from JWT: '{string.Join(", ", jsonToken.Audiences)}'"
                            );
                            Console.WriteLine($"Token Issuer from JWT: '{jsonToken.Issuer}'");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Could not parse token: {ex.Message}");
                        }

                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        IEnumerable<Claim>? claims = context.Principal?.Claims;
                        if (claims != null)
                        {
                            Console.WriteLine("Token validated successfully. Claims:");
                            IEnumerable<Claim> enumerable = claims.ToList();
                            foreach (Claim claim in enumerable) Console.WriteLine($"  {claim.Type}: {claim.Value}");

                            // Check roles specifically
                            var roleClaims = enumerable
                                .Where(c => c.Type is "role" or ClaimTypes.Role)
                                .ToList();
                            Console.WriteLine($"Role claims found: {roleClaims.Count}");
                            foreach (Claim roleClaim in roleClaims)
                                Console.WriteLine(
                                    $"  Role claim - Type: '{roleClaim.Type}', Value: '{roleClaim.Value}'"
                                );

                            // Ensure roles are available in both claim types for HotChocolate compatibility
                            if (context.Principal?.Identity is ClaimsIdentity identity)
                            {
                                var existingRoles = enumerable
                                    .Where(c => c.Type == "role" || c.Type == ClaimTypes.Role)
                                    .Select(c => c.Value)
                                    .Distinct()
                                    .ToList();

                                foreach (var role in existingRoles)
                                {
                                    // Add role claim with both "role" and ClaimTypes.Role if not already present
                                    if (!identity.HasClaim("role", role))
                                    {
                                        identity.AddClaim(new Claim("role", role));
                                        Console.WriteLine($"Added role claim: 'role' = '{role}'");
                                    }

                                    if (identity.HasClaim(ClaimTypes.Role, role)) continue;
                                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
                                    Console.WriteLine(
                                        $"Added role claim: '{ClaimTypes.Role}' = '{role}'"
                                    );
                                }
                            }
                        }

                        // Log token details for debugging
                        if (
                            context.SecurityToken
                            is JwtSecurityToken jwtToken
                        )
                        {
                            Console.WriteLine($"Token Issuer: '{jwtToken.Issuer}'");
                            Console.WriteLine(
                                $"Token Audiences: {string.Join(", ", jwtToken.Audiences)}"
                            );
                            Console.WriteLine($"Token Subject: '{jwtToken.Subject}'");
                        }

                        // Log user identity info
                        if (context.Principal?.Identity == null) return Task.CompletedTask;
                        Console.WriteLine(
                            $"User Identity Name: '{context.Principal.Identity.Name}'"
                        );
                        Console.WriteLine(
                            $"Is Authenticated: {context.Principal.Identity.IsAuthenticated}"
                        );

                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }
}

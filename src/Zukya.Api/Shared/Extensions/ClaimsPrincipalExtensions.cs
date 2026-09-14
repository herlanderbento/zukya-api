using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Zukya.Api.Shared.Configurations;

namespace Zukya.Api.Shared.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal? user)
    {
        if (user == null)
            return null;

        var claim = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                    ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(claim))
            return null;

        return Guid.TryParse(claim, out Guid id) ? id : null;
    }

    public static int GetUserRole(this ClaimsPrincipal? user)
    {
        if (user == null)
            return AccessLevels.Client;

        var claim = user.FindFirst("role")?.Value;

        if (string.IsNullOrWhiteSpace(claim))
            return AccessLevels.Client;

        return int.TryParse(claim, out var role) ? role : AccessLevels.Client;
    }
}

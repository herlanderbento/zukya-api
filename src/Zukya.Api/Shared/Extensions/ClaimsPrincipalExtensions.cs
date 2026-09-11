using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Zukya.Application.Common.Exceptions;

namespace Zukya.Api.Shared.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal? claimsPrincipal)
    {
        if (claimsPrincipal?.Identity?.IsAuthenticated != true)
            throw new UnauthorizedException("Authentication required.");

        Claim? userIdClaim =
            claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)
            ?? claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Sub);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            throw new UnauthorizedException("User ID not found in token.");

        return userId;
    }
}

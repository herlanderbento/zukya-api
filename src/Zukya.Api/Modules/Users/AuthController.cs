using MediatR;
using Microsoft.AspNetCore.Mvc;
using Zukya.Api.Shared.Presenters;
using Zukya.Api.Shared.Validators;
using Zukya.Application.UseCases.Users.Authenticate;
using Zukya.Application.UseCases.Users.Common;

namespace Zukya.Api.Modules.Users;

[ApiController]
[Route("/api/auth")]
public class AuthController(IMediator mediator, RequestValidator requestValidator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ApiPresenter<UserTokenOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Authenticate(
        [FromBody] AuthenticateInput request,
        CancellationToken cancellationToken
    )
    {
        requestValidator.Validate(request, cancellationToken);

        UserTokenOutput output = await mediator.Send(request, cancellationToken);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddHours(4)
        };

        Response.Cookies.Append("zukya_token", output.AccessToken, cookieOptions);

        return StatusCode(StatusCodes.Status200OK, new ApiPresenter<UserOutput>(output.User));
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Logout()
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(-1)
        };

        Response.Cookies.Append("zukya_token", string.Empty, cookieOptions);

        return Ok(new { message = "Logged out successfully" });
    }
}

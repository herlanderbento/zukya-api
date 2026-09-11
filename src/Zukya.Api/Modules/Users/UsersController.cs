using MediatR;
using Microsoft.AspNetCore.Mvc;
using Zukya.Api.Shared.Presenters;
using Zukya.Api.Shared.Validators;
using Zukya.Application.UseCases.Users.Common;
using Zukya.Application.UseCases.Users.CreateUser;

namespace Zukya.Api.Modules.Users;

[ApiController]
[Route("/api/users")]
public class UsersController(IMediator mediator, RequestValidator requestValidator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ApiPresenter<UserTokenOutput>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserInput request,
        CancellationToken cancellationToken
    )
    {
        requestValidator.Validate(request, cancellationToken);

        UserTokenOutput output = await mediator.Send(request, cancellationToken);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true, Secure = false, SameSite = SameSiteMode.Lax, Expires = DateTimeOffset.Now.AddHours(4)
        };

        Response.Cookies.Append("zukya_token", output.AccessToken, cookieOptions);

        return StatusCode(StatusCodes.Status201Created, new ApiPresenter<UserOutput>(output.User));
    }
}

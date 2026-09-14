using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zukya.Api.Modules.Users.Inputs;
using Zukya.Api.Shared.Configurations;
using Zukya.Api.Shared.Extensions;
using Zukya.Api.Shared.Presenters;
using Zukya.Api.Shared.Validators;
using Zukya.Application.UseCases.Users.Common;
using Zukya.Application.UseCases.Users.CreateUser;
using Zukya.Application.UseCases.Users.GetUser;
using Zukya.Application.UseCases.Users.UpdateUser;

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

    [HttpGet("/profile")]
    [Authorize(Policy = "ClientOrAbove")]
    [ProducesResponseType(typeof(ApiPresenter<UserOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Profile(
        CancellationToken cancellationToken
    )
    {
        Guid? userId = User.GetUserId();
        if (!userId.HasValue)
            return Unauthorized();

        UserOutput output = await mediator.Send(
            new GetUserInput(userId.Value),
            cancellationToken
        );
        return Ok(new ApiPresenter<UserOutput>(output));
    }

    [HttpPatch("{id:guid?}")]
    [Authorize(Policy = "ClientOrAbove")]
    [ProducesResponseType(typeof(ApiPresenter<UserOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid? id,
        [FromBody] UpdateUserApiInput request,
        CancellationToken cancellationToken
    )
    {
        Guid? loggedInUserId = User.GetUserId();
        var userLevel = User.GetUserRole();

        if (!loggedInUserId.HasValue)
            return Unauthorized();

        Guid targetId = loggedInUserId.Value;

        if (userLevel >= AccessLevels.Admin && id.HasValue) targetId = id.Value;

        var input = new UpdateUserInput(
            targetId,
            request.Name,
            request.Phone,
            request.TaxId
        );

        requestValidator.Validate(input, cancellationToken);
        UserOutput output = await mediator.Send(input, cancellationToken);

        return Ok(new ApiPresenter<UserOutput>(output));
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Zukya.Application.Common.Exceptions;
using Zukya.Domain.Shared.Exceptions;

namespace Zukya.Api.Shared.Filters;

public class ApiGlobalExceptionFilter(IHostEnvironment env) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var details = new ProblemDetails();
        Exception exception = context.Exception;

        switch (exception)
        {
            case EntityValidationException entityValidationException:
                details.Title = "One or more validation errors occurred";
                details.Status = StatusCodes.Status422UnprocessableEntity;
                details.Type = "UnprocessableEntity";
                details.Extensions["errors"] = entityValidationException.Message.Split(" | ");
                break;
            case NotFoundException:
                details.Title = "Not Found";
                details.Status = StatusCodes.Status404NotFound;
                details.Type = "NotFound";
                details.Detail = exception.Message;
                break;
            case ConflictException:
                details.Title = "Conflict";
                details.Status = StatusCodes.Status409Conflict;
                details.Type = "Conflict";
                details.Detail = exception.Message;
                break;
            default:
                details.Title = "An unexpected error occurred";
                details.Status = StatusCodes.Status500InternalServerError;
                details.Type = "UnexpectedError";
                details.Detail = exception.Message;

                if (env.IsDevelopment()) details.Extensions["stackTrace"] = exception.StackTrace;
                break;
        }

        context.HttpContext.Response.StatusCode = details.Status ?? StatusCodes.Status500InternalServerError;
        context.Result = new ObjectResult(details);
        context.ExceptionHandled = true;
    }
}

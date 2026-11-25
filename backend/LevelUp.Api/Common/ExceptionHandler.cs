using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace LevelUp.Api.Common;

/// <summary>
/// Oversætter vores egne exceptions til ProblemDetails-svar, så controllerne kan holdes simple.
/// </summary>
public class ExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Ikke fundet"),
            BusinessRuleException => (StatusCodes.Status409Conflict, "Handlingen kan ikke udføres"),
            _ => (0, string.Empty),
        };

        if (status == 0)
        {
            return false; // Ukendte fejl håndteres af standard-handleren (500)
        }

        httpContext.Response.StatusCode = status;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = exception.Message,
            },
            Exception = exception,
        });
    }
}

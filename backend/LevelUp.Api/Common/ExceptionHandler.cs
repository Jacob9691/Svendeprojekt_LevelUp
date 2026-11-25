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
        if (exception is not ApiException apiException)
        {
            return false; // Ukendte fejl håndteres af standard-handleren (500)
        }

        ProblemDetails problem = apiException is RequestValidationException validation
            ? new ValidationProblemDetails(validation.Errors)
            : new ProblemDetails();

        problem.Status = apiException.StatusCode;
        problem.Title = apiException.Title;
        problem.Detail = apiException.Message;

        if (apiException.Code is not null)
        {
            problem.Extensions["code"] = apiException.Code;
        }

        httpContext.Response.StatusCode = apiException.StatusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problem,
            Exception = exception,
        });
    }
}

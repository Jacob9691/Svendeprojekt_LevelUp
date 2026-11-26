using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LevelUp.Api.Common;

/// <summary>
/// Laver svaret når en request ikke består valideringen. Rydder op i fejl fra JSON-læsningen,
/// så frontenden får danske beskeder og feltnavne uden "$."-præfiks.
/// </summary>
public static class ValidationResponse
{
    public const string Title = "Et eller flere felter er ugyldige.";
    private const string InvalidValue = "Værdien er ugyldig.";

    public static IActionResult Create(ActionContext context)
    {
        var cleaned = new ModelStateDictionary();

        foreach (var (key, entry) in context.ModelState)
        {
            // Kun den første fejl pr. felt – et tomt felt skal bare give "skal udfyldes",
            // ikke også "er ikke en gyldig e-mailadresse"
            var error = entry.Errors.FirstOrDefault();
            if (error is null)
            {
                continue;
            }

            // JSON-fejl hedder f.eks. "$.confirmationMethod" – brug bare feltnavnet
            var field = key.StartsWith("$.") ? key[2..] : key;

            // Fejl fra JSON-læsningen har en teknisk engelsk besked (eller ingen)
            var message = error.Exception is not null || string.IsNullOrEmpty(error.ErrorMessage)
                ? InvalidValue
                : error.ErrorMessage;

            cleaned.AddModelError(field, message);
        }

        var problem = context.HttpContext.RequestServices
            .GetRequiredService<ProblemDetailsFactory>()
            .CreateValidationProblemDetails(context.HttpContext, cleaned, title: Title);

        return new BadRequestObjectResult(problem);
    }
}

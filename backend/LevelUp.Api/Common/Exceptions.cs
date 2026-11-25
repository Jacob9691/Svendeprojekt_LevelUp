namespace LevelUp.Api.Common;

/// <summary>
/// Basisklasse for fejl der skal sendes til klienten som ProblemDetails.
/// <see cref="Code"/> er en fast tekst frontenden kan reagere på, f.eks. "accountNotConfirmed".
/// </summary>
public abstract class ApiException(int statusCode, string title, string message, string? code = null)
    : Exception(message)
{
    public int StatusCode { get; } = statusCode;
    public string Title { get; } = title;
    public string? Code { get; } = code;
}

/// <summary>Ressourcen findes ikke (eller tilhører en anden bruger). 404.</summary>
public class NotFoundException(string message)
    : ApiException(StatusCodes.Status404NotFound, "Ikke fundet", message);

/// <summary>Handlingen strider mod en forretningsregel, f.eks. for få points. 409.</summary>
public class BusinessRuleException(string message, string? code = null)
    : ApiException(StatusCodes.Status409Conflict, "Handlingen kan ikke udføres", message, code);

/// <summary>Login eller kode er forkert. 401.</summary>
public class AuthenticationFailedException(string message, string? code = null)
    : ApiException(StatusCodes.Status401Unauthorized, "Login mislykkedes", message, code);

/// <summary>Brugeren er kendt, men må ikke fortsætte, f.eks. fordi kontoen er låst. 403.</summary>
public class AccessDeniedException(string message, string code)
    : ApiException(StatusCodes.Status403Forbidden, "Adgang nægtet", message, code);

/// <summary>Valideringsfejl der først opdages i en service, f.eks. fra Identity. 400.</summary>
public class RequestValidationException(IDictionary<string, string[]> errors)
    : ApiException(StatusCodes.Status400BadRequest, "Ugyldige data", "Et eller flere felter er ugyldige.")
{
    public IDictionary<string, string[]> Errors { get; } = errors;

    public RequestValidationException(string field, string error)
        : this(new Dictionary<string, string[]> { [field] = [error] })
    {
    }
}

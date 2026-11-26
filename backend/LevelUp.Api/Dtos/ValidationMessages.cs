namespace LevelUp.Api.Dtos;

/// <summary>
/// Danske fejlbeskeder til DataAnnotations. {0} er feltets navn fra [Display(Name = ...)],
/// {1} er f.eks. længden i MinLength/MaxLength.
/// </summary>
public static class ValidationMessages
{
    public const string Required = "{0} skal udfyldes.";
    public const string MinLength = "{0} skal være mindst {1} tegn.";
    public const string MaxLength = "{0} må højst være {1} tegn.";
    public const string Range = "{0} skal være mellem {1} og {2}.";
    public const string Email = "{0} er ikke en gyldig e-mailadresse.";
    public const string Phone = "{0} er ikke et gyldigt telefonnummer.";
    public const string Code = "Koden består af 6 cifre.";
    public const string TwoFactorTokenMissing = "Login-sessionen mangler. Log ind igen.";
}

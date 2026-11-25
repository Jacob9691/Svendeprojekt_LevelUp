using Microsoft.AspNetCore.Identity;

namespace LevelUp.Api.Services.Identity;

/// <summary>Danske fejlbeskeder for de Identity-fejl brugeren kan møde ved registrering.</summary>
public class DanishIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError DuplicateEmail(string email) =>
        new() { Code = nameof(DuplicateEmail), Description = "Der findes allerede en konto med denne e-mail." };

    public override IdentityError DuplicateUserName(string userName) =>
        new() { Code = nameof(DuplicateUserName), Description = "Der findes allerede en konto med denne e-mail." };

    public override IdentityError InvalidEmail(string? email) =>
        new() { Code = nameof(InvalidEmail), Description = "E-mailadressen er ugyldig." };

    public override IdentityError PasswordTooShort(int length) =>
        new() { Code = nameof(PasswordTooShort), Description = $"Adgangskoden skal være mindst {length} tegn." };

    public override IdentityError PasswordRequiresDigit() =>
        new() { Code = nameof(PasswordRequiresDigit), Description = "Adgangskoden skal indeholde mindst ét tal." };

    public override IdentityError PasswordRequiresLower() =>
        new() { Code = nameof(PasswordRequiresLower), Description = "Adgangskoden skal indeholde mindst ét lille bogstav." };

    public override IdentityError PasswordRequiresUpper() =>
        new() { Code = nameof(PasswordRequiresUpper), Description = "Adgangskoden skal indeholde mindst ét stort bogstav." };

    public override IdentityError PasswordRequiresNonAlphanumeric() =>
        new() { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "Adgangskoden skal indeholde mindst ét specialtegn." };

    public override IdentityError InvalidToken() =>
        new() { Code = nameof(InvalidToken), Description = "Koden er forkert eller udløbet." };
}

using LevelUp.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace LevelUp.Api.Services.Identity;

/// <summary>
/// En konto tæller som bekræftet, når enten e-mail eller telefonnummer er bekræftet med en kode.
/// Identity's standard kræver kun e-mail.
/// </summary>
public class AccountConfirmation : IUserConfirmation<AppUser>
{
    public Task<bool> IsConfirmedAsync(UserManager<AppUser> manager, AppUser user) =>
        Task.FromResult(user.EmailConfirmed || user.PhoneNumberConfirmed);
}

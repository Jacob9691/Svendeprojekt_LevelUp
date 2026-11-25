using System.Threading.RateLimiting;
using LevelUp.Api.Controllers;
using LevelUp.Api.Data;
using LevelUp.Api.Models;
using LevelUp.Api.Services;
using LevelUp.Api.Services.Identity;
using LevelUp.Api.Services.Messaging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace LevelUp.Api.Common;

public static class AuthSetup
{
    /// <summary>
    /// Opsætter ASP.NET Core Identity, JWT-autentificering, afsendelse af koder og rate limiting.
    /// </summary>
    public static IServiceCollection AddLevelUpAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<SmtpOptions>()
            .Bind(configuration.GetSection(SmtpOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Identity: brugere, password hashing, bekræftelseskoder, 2FA og lockout
        services
            .AddIdentityCore<AppUser>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;

                options.SignIn.RequireConfirmedAccount = true;

                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

                // 6-cifrede koder i stedet for lange links til e-mailbekræftelse
                options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders()
            .AddErrorDescriber<DanishIdentityErrorDescriber>();

        services.AddScoped<IUserConfirmation<AppUser>, AccountConfirmation>();

        // JWT: alle requests skal have et gyldigt access token, medmindre endpointet er [AllowAnonymous]
        var jwt = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false; // Behold "sub", "email" osv. som de står i tokenet
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = JwtTokenService.CreateSigningKey(jwt.SigningKey),
                    ClockSkew = TimeSpan.FromSeconds(30),
                    NameClaimType = JwtRegisteredClaimNames.Name,
                };
            });

        services.AddAuthorizationBuilder()
            .SetFallbackPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build());

        // Begrænser login/registrering pr. IP-adresse for at bremse brute force
        var authRequestsPerMinute = configuration.GetValue("RateLimiting:AuthRequestsPerMinute", 10);

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy(RateLimitPolicies.Auth, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = authRequestsPerMinute,
                        Window = TimeSpan.FromMinutes(1),
                    }));
        });

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddTransient<IEmailSender, SmtpEmailSender>();
        services.AddTransient<ISmsSender, LoggingSmsSender>();
        services.AddTransient<IVerificationCodeSender, VerificationCodeSender>();

        return services;
    }
}

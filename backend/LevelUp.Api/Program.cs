using System.Text.Json.Serialization;
using LevelUp.Api.Common;
using LevelUp.Api.Data;
using LevelUp.Api.Models;
using LevelUp.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

const string FrontendCorsPolicy = "Frontend";

builder.Services
    .AddControllers(options =>
    {
        // Danske beskeder når en værdi ikke kan læses, f.eks. tekst i et talfelt
        var messages = options.ModelBindingMessageProvider;
        messages.SetValueMustNotBeNullAccessor(_ => "Feltet skal udfyldes.");
        messages.SetAttemptedValueIsInvalidAccessor((value, field) => $"Værdien '{value}' er ugyldig for {field}.");
        messages.SetValueIsInvalidAccessor(value => $"Værdien '{value}' er ugyldig.");
        messages.SetValueMustBeANumberAccessor(field => $"{field} skal være et tal.");
        messages.SetMissingKeyOrValueAccessor(() => "Der mangler en værdi.");
        messages.SetMissingRequestBodyRequiredValueAccessor(() => "Der mangler data i requesten.");

        // Undgå den engelske "The request field is required." når JSON ikke kan læses
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    })
    .AddJsonOptions(options =>
    {
        // Enums sendes som tekst ("Completed") i stedet for tal
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        // Send ikke tekniske JSON-fejlbeskeder til klienten
        options.AllowInputFormatterExceptionMessages = false;
    })
    .ConfigureApiBehaviorOptions(options =>
        options.InvalidModelStateResponseFactory = ValidationResponse.Create);

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecurityTransformer>();
    options.AddOperationTransformer<BearerSecurityTransformer>();
});
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ExceptionHandler>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// Database – SQL Server. Connection string ligger i user-secrets (se README).
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' mangler. Sæt den med 'dotnet user-secrets' – se README.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity, JWT, e-mail/SMS og rate limiting – se Common/AuthSetup.cs
builder.Services.AddLevelUpAuth(builder.Configuration);

// Services
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IShopService, ShopService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IAchievementService, AchievementService>();
builder.Services.AddScoped<IChallengeService, ChallengeService>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();

    // Swagger UI på /swagger – viser dokumentet som MapOpenApi genererer
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "LevelUp API v1");
        options.DocumentTitle = "LevelUp API";
        options.EnablePersistAuthorization(); // Husk token ved genindlæsning
    });

    // Kør nye migrations automatisk og opret demo-data under udvikling
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await SeedData.SeedDevelopmentDataAsync(
        scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>(),
        app.Configuration);
}

app.UseHttpsRedirection();

app.UseCors(FrontendCorsPolicy);

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

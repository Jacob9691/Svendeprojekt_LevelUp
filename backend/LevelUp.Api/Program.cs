using System.Text.Json.Serialization;
using LevelUp.Api.Common;
using LevelUp.Api.Data;
using LevelUp.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

const string FrontendCorsPolicy = "Frontend";

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
        // Enums sendes som tekst ("Completed") i stedet for tal
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddOpenApi();
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

// Services
builder.Services.AddScoped<ICurrentUserService, DevCurrentUserService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IShopService, ShopService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IProfileService, ProfileService>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Swagger UI på /swagger – viser dokumentet som MapOpenApi genererer
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "LevelUp API v1");
        options.DocumentTitle = "LevelUp API";
    });

    // Kør nye migrations automatisk og opret demo-data under udvikling
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await SeedData.SeedDevelopmentDataAsync(db);
}

app.UseHttpsRedirection();

app.UseCors(FrontendCorsPolicy);

app.UseAuthorization();

app.MapControllers();

app.Run();

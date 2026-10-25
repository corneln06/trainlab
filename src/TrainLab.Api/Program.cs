using System.Security.Claims;
using System.Text.Json.Serialization;

using FluentValidation;

using Microsoft.AspNetCore.Identity;

using Scalar.AspNetCore;

using Serilog;

using TrainLab.Api.Errors;
using TrainLab.Api.Persistence;
using TrainLab.Infrastructure;
using TrainLab.Infrastructure.Persistence;
using TrainLab.Modules.Activities.Persistence;
using TrainLab.Modules.Analytics.Persistence;
using TrainLab.Modules.Import;
using TrainLab.Modules.Import.Persistence;
using TrainLab.Modules.Maps.Persistence;
using TrainLab.Modules.Profile.Athletes;
using TrainLab.Modules.Profile.Persistence;
using TrainLab.Modules.Profile.Users;
using TrainLab.SharedKernel.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<StorageOptions>()
    .Bind(builder.Configuration.GetSection(StorageOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<UploadOptions>()
    .Bind(builder.Configuration.GetSection(UploadOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' is missing.");

builder.Services.AddSerilog((services, logger) => logger
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString, name: "database");

builder.Services.AddOpenApi();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddDbContext<ActivitiesDbContext>(o => o.UseTrainLabPostgres(connectionString, ActivitiesDbContext.Schema));
builder.Services.AddDbContext<ProfileDbContext>(o => o.UseTrainLabPostgres(connectionString, ProfileDbContext.Schema));
builder.Services.AddDbContext<ImportDbContext>(o => o.UseTrainLabPostgres(connectionString, ImportDbContext.Schema));
builder.Services.AddDbContext<AnalyticsDbContext>(o => o.UseTrainLabPostgres(connectionString, AnalyticsDbContext.Schema));
builder.Services.AddDbContext<MapsDbContext>(o => o.UseTrainLabPostgres(connectionString, MapsDbContext.Schema));
builder.Services
    .AddValidatorsFromAssemblyContaining<UpdateAthleteProfileValidator>();

builder.Services.AddIdentityApiEndpoints<User>()
    .AddEntityFrameworkStores<ProfileDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

builder.Services.AddAuthorization();

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<IEventPublisher, InProcessEventPublisher>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await DatabaseMigrator.MigrateAllAsync(app.Services);

    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseSerilogRequestLogging();
app.UseExceptionHandler();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");

app.MapGroup("/api/auth").MapIdentityApi<User>();

var api = app.MapGroup("/api").RequireAuthorization();

api.MapPost("/auth/logout", async (SignInManager<User> signInManager) =>
{
    await signInManager.SignOutAsync();
    return TypedResults.NoContent();
});

api.MapGet("/me", (ClaimsPrincipal user) => TypedResults.Ok(new
{
    Id = user.FindFirstValue(ClaimTypes.NameIdentifier),
    Email = user.FindFirstValue(ClaimTypes.Email),
}));
api.MapProfileEndpoints();

app.Map("/api/{**rest}", () => TypedResults.NotFound());

app.MapFallbackToFile("index.html");

app.Logger.LogInformation("Starting TrainLab in {Environment}", app.Environment.EnvironmentName);
app.Run();
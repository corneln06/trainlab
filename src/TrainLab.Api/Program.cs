using Scalar.AspNetCore;

using Serilog;

using TrainLab.Api.Errors;
using TrainLab.Infrastructure;
using TrainLab.Infrastructure.Persistence;
using TrainLab.Modules.Activities.Persistence;
using TrainLab.Modules.Analytics.Persistence;
using TrainLab.Modules.Import;
using TrainLab.Modules.Import.Persistence;
using TrainLab.Modules.Maps.Persistence;
using TrainLab.Modules.Profile.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOptions<StorageOptions>()
    .Bind(builder.Configuration.GetSection(StorageOptions.SectionName))
    .ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<UploadOptions>()
    .Bind(builder.Configuration.GetSection(UploadOptions.SectionName))
    .ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddSerilog((services, logger) => logger.ReadFrom
    .Configuration(builder.Configuration).ReadFrom.Services(services).Enrich
    .FromLogContext());


builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
var connectionString = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string 'Default' is missing.");
    
    builder.Services.AddHealthChecks()
        .AddNpgSql(connectionString, name: "database");
    builder.Services.AddOpenApi();

builder.Services.AddDbContext<ProfileDbContext>(options => options.UseTrainLabPostgres(connectionString, ProfileDbContext.Schema));
builder.Services.AddDbContext<ImportDbContext>(options => options.UseTrainLabPostgres(connectionString, ImportDbContext.Schema));
builder.Services.AddDbContext<AnalyticsDbContext>(options => options.UseTrainLabPostgres(connectionString, AnalyticsDbContext.Schema));
builder.Services.AddDbContext<MapsDbContext>(options => options.UseTrainLabPostgres(connectionString, MapsDbContext.Schema));
builder.Services.AddDbContext<ActivitiesDbContext>(options => options.UseTrainLabPostgres(connectionString, ActivitiesDbContext.Schema));


var app = builder.Build();
app.UseSerilogRequestLogging();
app.UseExceptionHandler();

app.MapHealthChecks("/health");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/openapi/v1.json");
    app.MapScalarApiReference("/scalar");
    
}

if (app.Environment.IsDevelopment())
{
    await using var scope = app.Services.CreateAsyncScope();
    var services = scope.ServiceProvider;

    DbContext[] contexts =
    [
        services.GetRequiredService<ActivitiesDbContext>(),
        services.GetRequiredService<ProfileDbContext>(),
        services.GetRequiredService<ImportDbContext>(),
        services.GetRequiredService<AnalyticsDbContext>(),
        services.GetRequiredService<MapsDbContext>(),
    ];
    foreach (var context in contexts)
    {
        await context.Database.MigrateAsync();
    }
}
app.MapGet("/", () => "Hello World!");


app.Logger.LogInformation("Starting TrainLab in {Environment}", app.Environment.EnvironmentName);
app.Run();
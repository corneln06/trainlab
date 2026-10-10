using Scalar.AspNetCore;

using Serilog;

using TrainLab.Api.Errors;
using TrainLab.Infrastructure;
using TrainLab.Modules.Import;

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



var app = builder.Build();
app.UseSerilogRequestLogging();
app.UseExceptionHandler();

app.MapHealthChecks("/health");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/openapi/v1.json");
    app.MapScalarApiReference("/scalar");
    
}
app.MapGet("/", () => "Hello World!");

app.Logger.LogInformation("Starting TrainLab in {Environment}", app.Environment.EnvironmentName);
app.Run();
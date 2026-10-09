using Serilog;

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


var app = builder.Build();

app.UseSerilogRequestLogging();

app.Logger.LogInformation("Starting TrainLab in {Environment}", app.Environment.EnvironmentName);
app.Run();
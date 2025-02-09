using WorkerService.Configuration;

var builder = Host.CreateApplicationBuilder(args);
var config = AppSettingsConfig.LoadConfiguration(builder.Configuration);
builder.Services.AddSingleton(config);
builder.ConfigureAmazonSqsService(config);

builder.ConfigureDbContext();
builder.ConfigureServices();

var app = builder.Build();
app.Run();

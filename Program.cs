using Serilog;
using Serilog.Events;
using TranslationService;
using TranslationService.Models;
using TranslationService.Services;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.File(@"C:\temp\workerservice\LogFile.txt")
    .CreateLogger();

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddTransient<ISettingsReader, SettingsReader>();
    })
    .Build();
try
{
    Log.Information("Starting the service");
    Log.Information("Settings Loaded");
    host.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
using fitnes.bot;
using fitnes.bot.Extension.DI;
using fitnes.Domain.Constants.Bot;
using fitnes.Domain.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;
using Telegram.Bot;

AppDomain.CurrentDomain.UnhandledException += (_, args) =>
{
    Log.Fatal(args.ExceptionObject as Exception, "Unhandled domain exception");
    Log.CloseAndFlush();
    Environment.ExitCode = 1;
};

TaskScheduler.UnobservedTaskException += (_, args) =>
{
    Log.Fatal(args.Exception, "Unobserved task exception");
    args.SetObserved();
};

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: LoggingConstants.ConsoleOutputTemplate)
    .WriteTo.File(
        Path.Combine(LoggingConstants.LogDirectory, LoggingConstants.LogFileName),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: LoggingConstants.RetainedFileCount,
        outputTemplate: LoggingConstants.FileOutputTemplate)
    .CreateLogger();

SelfLog.Enable(msg => Console.Error.WriteLine($"[Serilog SelfLog] {msg}"));

Log.Information("Starting application...");

try
{
    HostApplicationBuilder builder = new HostApplicationBuilder(args);

    builder.Services.AddSerilog();

    builder.Services.AddSingleton<ITelegramBotClient>(sp =>
    {
        TelegramBotClient client = new(builder.Configuration[$"{TelegramBotOptions.SectionName}:Token"] ?? throw new ArgumentNullException());
        client.Timeout = TimeSpan.FromSeconds(BotConstants.TelegramPollingTimeoutSeconds);
        return client;
    });
    builder.Services.AddOptions(builder.Configuration);
    builder.Services.AddSingleton<fitnes.bot.Services.PollingHealthState>();
    builder.Services.AddHostedService<BotWorker>();
    builder.Services.AddHostedService<fitnes.bot.Services.PollingHealthMonitor>();
    builder.Services.AddServices();
    builder.Services.AddBotHandlers();
    builder.Services.AddPipeline();
    builder.Services.AddLocalization();
    builder.Services.AddPersistence(builder.Configuration);

    var app = builder.Build();

    Log.Information("Application built, starting host...");
    app.Run();
    Log.Information("Host stopped");

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");

    return 1;
}
finally
{
    Log.CloseAndFlush();
}

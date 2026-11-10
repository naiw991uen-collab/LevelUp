using Serilog;
using Serilog.Events;

namespace LevelUp.PL.Extentions.Logging;

public static class LoggingExtensions
{
    private const string OutputTemplate =
        "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}";

    public static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder)
    {
        var logsFolder = Path.Combine(AppContext.BaseDirectory, "Logs");

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information)
                .WriteTo.File(Path.Combine(logsFolder, "Success", "success-.txt"),
                    rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30, outputTemplate: OutputTemplate))
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning)
                .WriteTo.File(Path.Combine(logsFolder, "Warnings", "warning-.txt"),
                    rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30, outputTemplate: OutputTemplate))
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(e => e.Level >= LogEventLevel.Error)
                .WriteTo.File(Path.Combine(logsFolder, "Errors", "error-.txt"),
                    rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30, outputTemplate: OutputTemplate))
            .CreateLogger();

        builder.Host.UseSerilog();

        return builder;
    }
}

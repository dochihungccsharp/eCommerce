using Microsoft.Extensions.Hosting;
using Serilog;

namespace eCommerce.Shared.Serilog;

public static class SeriLogger
{
    public static Action<HostBuilderContext, LoggerConfiguration> Configure =>
        (context, configuration) =>
        {
            var applicationName = context.HostingEnvironment.ApplicationName.ToLower().Replace(".", "-");
            var environmentName = context.HostingEnvironment.EnvironmentName ?? "Development";

            configuration
                .WriteTo.Debug()
                .WriteTo.Console(outputTemplate:
                    "[{Timestamp: HH:mm:ss} {Level}] {SourceContext} {NewLine} {Message:lj}{NewLine}{Exception}{NewLine}")
                .WriteTo.File(
                    $@"logs/{applicationName}-{environmentName}.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}",
                    fileSizeLimitBytes: 10485760, // 10 MB
                    rollOnFileSizeLimit: true,
                    retainedFileCountLimit: 30
                )
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProperty("Environment", environmentName)
                .Enrich.WithProperty("Application", applicationName)
                .ReadFrom.Configuration(context.Configuration);
        };
}
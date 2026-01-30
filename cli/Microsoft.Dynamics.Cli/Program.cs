using Microsoft.Dynamics.Cli;
using Microsoft.Dynamics.Core.Configuration;
using Microsoft.Dynamics.Core.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

if (args.Length > 0 && args[0] == "--debug")
{
    var processPath = Environment.ProcessPath;
    Console.WriteLine($"Process Path: {processPath}");

    var realExePath = ExecutablePathResolver.GetRealExecutablePath();
    Console.WriteLine($"Executable directory: {realExePath}");

    var baseDirectory = AppContext.BaseDirectory; ;
    Console.WriteLine($"BaseDirectory: {baseDirectory}");

    Console.WriteLine($"DOTNET_ENVIRONMENT: {Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}");

    return 0;
}

var contentRootPath = Path.GetDirectoryName(ExecutablePathResolver.GetRealExecutablePath()) ?? Environment.CurrentDirectory;
var builder = Host.CreateApplicationBuilder(
    new HostApplicationBuilderSettings 
    { 
        ContentRootPath = contentRootPath, 
        Args = args 
    });

#if DEBUG
builder.Environment.EnvironmentName = Environments.Development;
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
#endif

builder.Logging.ClearProviders();
builder.Services.AddSerilog((context, conf) => { conf.ReadFrom.Configuration(builder.Configuration); });
builder.Services.AddSingleton<MainProcess>();
var host = builder.Build();

// Register upstream dependencies
var loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
LoggerManager.Initiate(loggerFactory);

var configuration = host.Services.GetRequiredService<IConfiguration>();
ConfigManager.Initiate(configuration);

// Run the main process
var serviceProvider = builder.Services.BuildServiceProvider();
var process = serviceProvider.GetRequiredService<MainProcess>();
var exitCode = await process.RunAsync(args);

Environment.Exit(exitCode);

return 0;
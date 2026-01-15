using Microsoft.Dynamics.Cli;
using Microsoft.Dynamics.Core.Configuration;
using Microsoft.Dynamics.Core.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();
builder.Services.AddSerilog((context, conf) => { conf.ReadFrom.Configuration(builder.Configuration); });
builder.Services.AddSingleton<MainProcess>();
//builder.Services.AddSingleton<CliProcess>();
//builder.Services.AddSingleton<ITypeRegistrar, TypeRegistrar>();
var host = builder.Build();

// Register upstream dependencies
var loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
LoggerManager.Initiate(loggerFactory);

var configuration = host.Services.GetRequiredService<IConfiguration>();
ConfigManager.Initiate(configuration);

// Run the main process
var serviceProvider = builder.Services.BuildServiceProvider();
var process = serviceProvider.GetRequiredService<MainProcess>();
//var process = serviceProvider.GetRequiredService<CliProcess>();
var exitCode = await process.RunAsync(args);

Environment.Exit(exitCode);

return 0;
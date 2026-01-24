using Microsoft.Dynamics.Client;
using Microsoft.Dynamics.Core.Logging;
using Microsoft.Extensions.Logging;
using Partner.Center.Cli;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Microsoft.Dynamics.Cli
{
    internal class MainProcess
    {
        private static ILogger<MainProcess> _logger;

        public MainProcess(ILogger<MainProcess> logger)
        {
            _logger = logger;
        }

        internal async Task<int> RunAsync(string[] args)
        {
            _logger.LogInformation("Current process folder: {folder}", Environment.CurrentDirectory);
            _logger.LogInformation("Current thread id: {threadId}", Environment.ProcessId);

            var app = new CommandApp();

            app.Configure(config =>
            {
                config.SetApplicationName("mdcli");
                config.SetApplicationVersion("1.0");

#if DEBUG
                config.PropagateExceptions();
#endif

                // Configure account commands
                config.AddBranch("account", account =>
                {
                    account.AddCommand<Commands.Account.SearchAccountCommand>("search")
                        .WithDescription("Search for accounts by name")
                        .WithExample(new[] { "account", "search", "--name", "Contoso" })
                        .WithExample(new[] { "account", "search", "-n", "Microsoft", "--top", "5" })
                        .WithExample(new[] { "account", "search", "-n", "Corp", "--contains" });
                });

                config.ValidateExamples();

                config.SetExceptionHandler((ex, resolver) =>
                {
                    AnsiConsole.MarkupInterpolated($"[red]{ex.Message}[/]");
                    return 1;
                });

            });

            try
            {
                return await app.RunAsync(args);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupInterpolated($"[red]{ex.Message}[/]");
                return -1;
            }
        }
    }
}
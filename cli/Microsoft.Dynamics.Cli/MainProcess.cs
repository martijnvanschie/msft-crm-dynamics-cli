using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Microsoft.Dynamics.Cli
{
    internal class MainProcess
    {
        protected readonly ILogger<MainProcess> _logger;

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

                // Configure opportunity commands
                config.AddBranch("opportunity", opportunity =>
                {
                    opportunity.AddCommand<Commands.Opportunity.SearchOpportunityCommand>("search")
                        .WithDescription("Search for opportunities by name")
                        .WithExample(new[] { "opportunity", "search", "--name", "Contoso" })
                        .WithExample(new[] { "opportunity", "search", "-n", "Microsoft", "--top", "5" })
                        .WithExample(new[] { "opportunity", "search", "-n", "Cloud", "--contains", "--include-closed" });

                    opportunity.AddCommand<Commands.Opportunity.GetOpportunitiesByAccountCommand>("by-account")
                        .WithDescription("Get all opportunities for a specific account")
                        .WithExample(new[] { "opportunity", "by-account", "12345678-1234-1234-1234-123456789abc" })
                        .WithExample(new[] { "opportunity", "by-account", "12345678-1234-1234-1234-123456789abc", "--json" });

                    opportunity.AddCommand<Commands.Opportunity.GetOpportunitiesByAccountNameCommand>("by-account-name")
                        .WithDescription("Get opportunities for an account by searching by name")
                        .WithExample(new[] { "opportunity", "by-account-name", "Contoso" })
                        .WithExample(new[] { "opportunity", "by-account-name", "Microsoft", "--include-closed" })
                        .WithExample(new[] { "opportunity", "by-account-name", "Corp", "--contains" });
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
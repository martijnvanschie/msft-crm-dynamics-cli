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
                        .WithExample(new[] { "account", "search", "-n", "Corp", "--contains" })
                        .WithExample(new[] { "account", "search", "-n", "Contoso", "-o", "json" });
                });

                // Configure opportunity commands
                config.AddBranch("opportunity", opportunity =>
                {
                    opportunity.AddCommand<Commands.Opportunity.GetOpportunityCommand>("show")
                        .WithDescription("Get a specific opportunity by id")
                        .WithExample(new[] { "opportunity", "show", "--id", "12345678-1234-1234-1234-123456789abc" })
                        .WithExample(new[] { "opportunity", "show", "--id", "12345678-1234-1234-1234-123456789abc", "-o", "json" });

                    opportunity.AddCommand<Commands.Opportunity.SearchOpportunityCommand>("search")
                        .WithDescription("Search for opportunities by name, opportunity number or id")
                        .WithExample(new[] { "opportunity", "search", "--id", "12345678-1234-1234-1234-123456789abc" })
                        .WithExample(new[] { "opportunity", "search", "--number", "2023-3761" })
                        .WithExample(new[] { "opportunity", "search", "--number", "2023", "--include-closed", "--top", "10" })
                        .WithExample(new[] { "opportunity", "search", "--number", "3761", "--contains", "-o", "json" })
                        .WithExample(new[] { "opportunity", "search", "--name", "Contoso" })
                        .WithExample(new[] { "opportunity", "search", "-n", "Microsoft", "--top", "5" })
                        .WithExample(new[] { "opportunity", "search", "-n", "Cloud", "--contains", "--include-closed" })
                        .WithExample(new[] { "opportunity", "search", "-n", "Azure", "--output", "json" });

                    opportunity.AddCommand<Commands.Opportunity.GetOpportunitiesByAccountCommand>("by-account")
                        .WithDescription("Get all opportunities for a specific account")
                        .WithExample(new[] { "opportunity", "by-account", "12345678-1234-1234-1234-123456789abc" })
                        .WithExample(new[] { "opportunity", "by-account", "12345678-1234-1234-1234-123456789abc", "--include-closed" })
                        .WithExample(new[] { "opportunity", "by-account", "12345678-1234-1234-1234-123456789abc", "-o", "json" });

                    opportunity.AddCommand<Commands.Opportunity.GetOpportunitiesByAccountNameCommand>("by-account-name")
                        .WithDescription("Get opportunities for an account by searching by name")
                        .WithExample(new[] { "opportunity", "by-account-name", "Contoso" })
                        .WithExample(new[] { "opportunity", "by-account-name", "Microsoft", "--include-closed" })
                        .WithExample(new[] { "opportunity", "by-account-name", "Corp", "--contains" })
                        .WithExample(new[] { "opportunity", "by-account-name", "Azure", "--output", "json" });

                    opportunity.AddCommand<Commands.Opportunity.GetNewOpportunitiesCommand>("new")
                        .WithDescription("Get newly created opportunities since a specific date")
                        .WithExample(new[] { "opportunity", "new" })
                        .WithExample(new[] { "opportunity", "new", "--from", "2024-01-01" })
                        .WithExample(new[] { "opportunity", "new", "-f", "2024-01-15", "--top", "100" })
                        .WithExample(new[] { "opportunity", "new", "--from", "2024-01-01", "--include-closed", "-o", "json" });
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
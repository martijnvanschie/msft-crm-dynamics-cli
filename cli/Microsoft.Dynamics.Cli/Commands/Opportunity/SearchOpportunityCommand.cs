using Microsoft.Dynamics.Client;
using Microsoft.Dynamics.Core.Logging;
using Microsoft.Extensions.Logging;
using Partner.Center.Cli;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Text.Json;

namespace Microsoft.Dynamics.Cli.Commands.Opportunity
{
    internal class SearchOpportunityCommand : AsyncCommand<SearchOpportunityCommand.Settings>
    {
        protected readonly static ILogger<SearchOpportunityCommand> _logger = LoggerManager.GetLogger<SearchOpportunityCommand>();

        public SearchOpportunityCommand()
        {
        }

        public sealed class Settings : CommandSettings
        {
            [CommandOption("-n|--name <NAME>")]
            [Description("The name or partial name of the opportunity to search for")]
            public string? Name { get; set; }

            [CommandOption("-t|--top <COUNT>")]
            [Description("Maximum number of results to return (default: 20)")]
            [DefaultValue(20)]
            public int Top { get; set; } = 20;

            [CommandOption("-c|--contains")]
            [Description("Use 'contains' search instead of 'starts with' (default is starts with)")]
            [DefaultValue(false)]
            public bool UseContains { get; set; } = false;

            [CommandOption("--include-closed")]
            [Description("Include closed opportunities in the results")]
            [DefaultValue(false)]
            public bool IncludeClosed { get; set; } = false;

            [CommandOption("-j|--json")]
            [Description("Output raw JSON response")]
            [DefaultValue(false)]
            public bool OutputJson { get; set; } = false;
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(settings.Name))
            {
                AnsiConsole.MarkupLine("[red]Error: --name parameter is required[/]");
                return 1;
            }

            TokenProvider.clientSecret = CliContext.ClientSecret;
            TokenProvider.clientId = CliContext.ClientId;
            TokenProvider.tenantId = CliContext.TenantId;

            try
            {
                string searchMode = settings.UseContains ? "containing" : "starting with";

                await AnsiConsole.Status()
                    .Spinner(Spinner.Known.Dots)
                    .StartAsync($"Searching for opportunities {searchMode} '{settings.Name}'...", async ctx =>
                    {
                        DynamicsOpportunitiesClient opportunitiesClient = new DynamicsOpportunitiesClient();
                        var result = await opportunitiesClient.GetOpportunitiesByName(settings.Name, settings.Top, !settings.UseContains);

                        ctx.Status("Processing results...");

                        var opportunities = settings.IncludeClosed
                            ? result.Value.OrderBy(o => o.StateCode).ToList()
                            : result.Value.Where(o => o.StateCode == 0).ToList();

                        if (opportunities.Count == 0)
                        {
                            _logger.LogInformation("No opportunities found {searchMode} '{Name}'", searchMode, settings.Name);
                            AnsiConsole.WriteLine();
                            AnsiConsole.MarkupLine($"[yellow]No opportunities found {searchMode} '{settings.Name}'[/]");
                            return;
                        }

                        if (settings.OutputJson)
                        {
                            var jsonOptions = new JsonSerializerOptions
                            {
                                WriteIndented = true
                            };
                            AnsiConsole.WriteLine(JsonSerializer.Serialize(opportunities, jsonOptions));
                        }
                        else
                        {
                            AnsiConsole.WriteLine();
                            var opportunityType = settings.IncludeClosed ? "Opportunities" : "Open opportunities";
                            AnsiConsole.MarkupLine($"[green]Search results for {opportunityType.ToLower()} {searchMode} '{settings.Name}':[/]");
                            AnsiConsole.WriteLine();

                            OpportunityTableRenderer.RenderOpportunitiesListTable(opportunities);

                            AnsiConsole.WriteLine();
                            AnsiConsole.MarkupLine($"[dim]Total results: {opportunities.Count}[/]");
                        }
                    });

                return 0;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error occurred while searching for opportunities");
                AnsiConsole.MarkupLine($"[red]HTTP Error: {ex.Message}[/]");
                return 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching for opportunities");
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
                return 1;
            }
        }
    }
}

using Microsoft.Dynamics.Client;
using Microsoft.Dynamics.Cli.Utils;
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

            [CommandOption("--number <NUMBER>")]
            [Description("The opportunity number (e.g. 2023-3761) to search for")]
            public string? Number { get; set; }

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

            [CommandOption("-o|--output <FORMAT>")]
            [Description("Output format: json or table (default: table)")]
            [DefaultValue("table")]
            public string Output { get; set; } = "table";
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
        {
            bool byNumber = !string.IsNullOrWhiteSpace(settings.Number);
            if (!byNumber && string.IsNullOrWhiteSpace(settings.Name))
            {
                AnsiConsole.MarkupLine("[red]Error: --name or --number parameter is required[/]");
                return 1;
            }

            string searchTerm = byNumber ? settings.Number! : settings.Name!;

            TokenProvider.clientSecret = CliContext.ClientSecret;
            TokenProvider.clientId = CliContext.ClientId;
            TokenProvider.tenantId = CliContext.TenantId;

            try
            {
                string searchMode = settings.UseContains ? "containing" : "starting with";

                await AnsiConsole.Status()
                    .Spinner(Spinner.Known.Dots)
                    .StartAsync($"Searching for opportunities {searchMode} '{Markup.Escape(searchTerm)}'...", async ctx =>
                    {
                        OpportunitiesClient opportunitiesClient = new OpportunitiesClient();
                        var requestParameters = new OpportunitiesRequestParameters()
                        {
                            Top = settings.Top,
                            UseStartsWith = !settings.UseContains,
                            IncludeClosed = settings.IncludeClosed
                        };
                        var result = byNumber
                            ? await opportunitiesClient.GetOpportunitiesByNumber(searchTerm, requestParameters)
                            : await opportunitiesClient.GetOpportunitiesByName(searchTerm, requestParameters);

                        ctx.Status("Processing results...");

                        var opportunities = result.Value.OrderBy(o => o.StateCode).ThenByDescending(o => o.CreatedOn).ToList();

                        if (opportunities.Count == 0)
                        {
                            _logger.LogInformation("No opportunities found {searchMode} '{Name}'", searchMode, searchTerm);
                            AnsiConsole.WriteLine();
                            AnsiConsole.MarkupLine($"[yellow]No opportunities found {searchMode} '{Markup.Escape(searchTerm)}'[/]");
                            return;
                        }

                        ctx.Status("Loading allocations...");
                        await opportunitiesClient.LoadAllocationsAsync(opportunities);

                        if (string.Equals(settings.Output, "table", StringComparison.OrdinalIgnoreCase))
                        {
                            AnsiConsole.WriteLine();
                            var opportunityType = settings.IncludeClosed ? "Opportunities" : "Open opportunities";
                            AnsiConsole.MarkupLine($"[green]Search results for {opportunityType.ToLower()} {searchMode} '{Markup.Escape(searchTerm)}':[/]");
                            AnsiConsole.WriteLine();

                            SpectreHelper.RenderOpportunitiesListTable(opportunities);

                            AnsiConsole.WriteLine();
                            AnsiConsole.MarkupLine($"[dim]Total results: {opportunities.Count}[/]");
                        }
                        else
                        {
                            var jsonOptions = new JsonSerializerOptions
                            {
                                WriteIndented = true
                            };
                            AnsiConsole.WriteLine(JsonSerializer.Serialize(opportunities, jsonOptions));
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

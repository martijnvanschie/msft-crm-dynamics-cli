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
    internal class GetNewOpportunitiesCommand : AsyncCommand<GetNewOpportunitiesCommand.Settings>
    {
        protected readonly static ILogger<GetNewOpportunitiesCommand> _logger = LoggerManager.GetLogger<GetNewOpportunitiesCommand>();

        public GetNewOpportunitiesCommand()
        {
        }

        public sealed class Settings : CommandSettings
        {
            [CommandOption("-f|--from <DATE>")]
            [Description("Get opportunities created on or after this date (default: 7 days ago). Format: yyyy-MM-dd")]
            public string? FromDate { get; set; }

            [CommandOption("-t|--top <COUNT>")]
            [Description("Maximum number of results to return (default: 50)")]
            [DefaultValue(50)]
            public int Top { get; set; } = 50;

            [CommandOption("-c|--include-closed")]
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
            DateTime fromDate;

            if (string.IsNullOrWhiteSpace(settings.FromDate))
            {
                fromDate = DateTime.UtcNow.AddDays(-7);
                _logger.LogInformation("No from date specified, using default: 7 days ago");
            }
            else
            {
                if (!DateTime.TryParse(settings.FromDate, out fromDate))
                {
                    AnsiConsole.MarkupLine("[red]Error: Invalid date format. Please use yyyy-MM-dd format[/]");
                    return 1;
                }
                fromDate = DateTime.SpecifyKind(fromDate, DateTimeKind.Utc);
            }

            if (!string.Equals(settings.Output, "json", StringComparison.OrdinalIgnoreCase) && 
                !string.Equals(settings.Output, "table", StringComparison.OrdinalIgnoreCase))
            {
                AnsiConsole.MarkupLine("[red]Error: --output must be either 'json' or 'table'[/]");
                return 1;
            }

            TokenProvider.clientSecret = CliContext.ClientSecret;
            TokenProvider.clientId = CliContext.ClientId;
            TokenProvider.tenantId = CliContext.TenantId;

            try
            {
                await AnsiConsole.Status()
                    .Spinner(Spinner.Known.Dots)
                    .StartAsync($"Retrieving opportunities created since {fromDate:yyyy-MM-dd}...", async ctx =>
                    {
                        OpportunitiesClient opportunitiesClient = new OpportunitiesClient();
                        var result = await opportunitiesClient.GetNewOpportunities(
                            fromDate,
                            new OpportunitiesRequestParameters()
                            {
                                Top = settings.Top,
                                IncludeClosed = settings.IncludeClosed
                            });

                        ctx.Status("Processing results...");

                        var opportunities = result.Value.OrderByDescending(o => o.CreatedOn).ToList();

                        if (opportunities.Count == 0)
                        {
                            _logger.LogInformation("No opportunities found created since {FromDate}", fromDate);
                            AnsiConsole.WriteLine();
                            AnsiConsole.MarkupLine($"[yellow]No opportunities found created since {fromDate:yyyy-MM-dd}[/]");
                            return;
                        }

                        if (string.Equals(settings.Output, "table", StringComparison.OrdinalIgnoreCase))
                        {
                            AnsiConsole.WriteLine();
                            var opportunityType = settings.IncludeClosed ? "Opportunities" : "Open opportunities";
                            AnsiConsole.MarkupLine($"[green]{opportunityType} created since {fromDate:yyyy-MM-dd}:[/]");
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
                _logger.LogError(ex, "HTTP error occurred while retrieving new opportunities");
                AnsiConsole.MarkupLine($"[red]HTTP Error: {ex.Message}[/]");
                return 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving new opportunities");
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
                return 1;
            }
        }
    }
}

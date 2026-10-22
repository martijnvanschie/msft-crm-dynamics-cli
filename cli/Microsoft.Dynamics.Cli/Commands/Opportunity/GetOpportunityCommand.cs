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
    internal class GetOpportunityCommand : AsyncCommand<GetOpportunityCommand.Settings>
    {
        protected readonly static ILogger<GetOpportunityCommand> _logger = LoggerManager.GetLogger<GetOpportunityCommand>();

        public sealed class Settings : CommandSettings
        {
            [CommandOption("--id <ID>")]
            [Description("The id (GUID) of the opportunity to retrieve")]
            public string? Id { get; set; }

            [CommandOption("-o|--output <FORMAT>")]
            [Description("Output format: json or table (default: table)")]
            [DefaultValue("table")]
            public string Output { get; set; } = "table";
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(settings.Id))
            {
                AnsiConsole.MarkupLine("[red]Error: --id parameter is required[/]");
                return 1;
            }

            if (!Guid.TryParse(settings.Id, out var opportunityId))
            {
                AnsiConsole.MarkupLine("[red]Error: --id must be a valid GUID[/]");
                return 1;
            }

            TokenProvider.clientSecret = CliContext.ClientSecret;
            TokenProvider.clientId = CliContext.ClientId;
            TokenProvider.tenantId = CliContext.TenantId;

            try
            {
                int exitCode = 0;

                await AnsiConsole.Status()
                    .Spinner(Spinner.Known.Dots)
                    .StartAsync($"Retrieving opportunity {opportunityId}...", async ctx =>
                    {
                        OpportunitiesClient client = new OpportunitiesClient();
                        var result = await client.GetOpportunitiesById(opportunityId.ToString());
                        var opportunity = result.Value.FirstOrDefault();

                        if (opportunity is null)
                        {
                            _logger.LogInformation("No opportunity found with id {Id}", opportunityId);
                            AnsiConsole.MarkupLine($"[yellow]No opportunity found with id '{opportunityId}'[/]");
                            exitCode = 1;
                            return;
                        }

                        ctx.Status("Loading allocations...");
                        var opportunities = new List<Client.Model.OpportunityDTO> { opportunity };
                        await client.LoadAllocationsAsync(opportunities);

                        if (string.Equals(settings.Output, "table", StringComparison.OrdinalIgnoreCase))
                        {
                            AnsiConsole.WriteLine();
                            SpectreHelper.RenderOpportunitiesListTable(opportunities, true);
                        }
                        else
                        {
                            var jsonOptions = new JsonSerializerOptions
                            {
                                WriteIndented = true
                            };
                            AnsiConsole.WriteLine(JsonSerializer.Serialize(opportunity, jsonOptions));
                        }
                    });

                return exitCode;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error occurred while retrieving opportunity");
                AnsiConsole.MarkupLine($"[red]HTTP Error: {ex.Message}[/]");
                return 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving opportunity");
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
                return 1;
            }
        }
    }
}

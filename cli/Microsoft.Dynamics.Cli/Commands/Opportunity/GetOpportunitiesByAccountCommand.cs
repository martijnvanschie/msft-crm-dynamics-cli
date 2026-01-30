using Microsoft.Dynamics.Client;
using Partner.Center.Cli;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Text.Json;

namespace Microsoft.Dynamics.Cli.Commands.Opportunity
{
    internal class GetOpportunitiesByAccountCommand : AsyncCommand<GetOpportunitiesByAccountCommand.Settings>
    {
        public GetOpportunitiesByAccountCommand()
        {
        }

        public sealed class Settings : CommandSettings
        {
            [CommandArgument(0, "<ACCOUNT_ID>")]
            [Description("The GUID of the account to get opportunities for")]
            public string AccountId { get; set; } = string.Empty;

            [CommandOption("-j|--json")]
            [Description("Output raw JSON response")]
            [DefaultValue(false)]
            public bool OutputJson { get; set; } = false;

            [CommandOption("-c|--include-closed")]
            [Description("Include closed opportunities in the results")]
            [DefaultValue(false)]
            public bool IncludeClosed { get; set; } = false;
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(settings.AccountId))
            {
                AnsiConsole.MarkupLine("[red]Error: Account ID is required[/]");
                return 1;
            }

            if (!Guid.TryParse(settings.AccountId, out _))
            {
                AnsiConsole.MarkupLine("[red]Error: Account ID must be a valid GUID[/]");
                return 1;
            }

            TokenProvider.clientSecret = CliContext.ClientSecret;
            TokenProvider.clientId = CliContext.ClientId;
            TokenProvider.tenantId = CliContext.TenantId;

            try
            {
                await AnsiConsole.Status()
                    .Spinner(Spinner.Known.Dots)
                    .StartAsync($"Retrieving opportunities for account {settings.AccountId}...", async ctx =>
                    {
                        DynamicsOpportunitiesClient client = new DynamicsOpportunitiesClient();
                        var result = await client.GetOpportunitiesByAccount(settings.AccountId, 20, settings.IncludeClosed);

                        ctx.Status("Processing results...");

                        var opportunities = result.Value.OrderBy(o => o.StateCode).ThenByDescending(o => o.EstimatedCloseDate).ToList();

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
                            AnsiConsole.MarkupLine($"[green]{opportunityType} for account {settings.AccountId}:[/]");
                            AnsiConsole.WriteLine();
                            
                            OpportunityTableRenderer.RenderOpportunitiesTable(opportunities);
                        }
                    });

                return 0;
            }
            catch (HttpRequestException ex)
            {
                AnsiConsole.MarkupLine($"[red]HTTP Error: {ex.Message}[/]");
                return 1;
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
                return 1;
            }
        }
    }
}

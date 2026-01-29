using Microsoft.Dynamics.Client;
using Partner.Center.Cli;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Text.Json;

namespace Microsoft.Dynamics.Cli.Commands.Opportunity
{
    internal class GetOpportunitiesByAccountNameCommand : AsyncCommand<GetOpportunitiesByAccountNameCommand.Settings>
    {
        public GetOpportunitiesByAccountNameCommand()
        {
        }

        public sealed class Settings : CommandSettings
        {
            [CommandArgument(0, "<ACCOUNT_NAME>")]
            [Description("The name or partial name of the account to search for")]
            public string AccountName { get; set; } = string.Empty;

            [CommandOption("-j|--json")]
            [Description("Output raw JSON response")]
            [DefaultValue(false)]
            public bool OutputJson { get; set; } = false;

            [CommandOption("-c|--include-closed")]
            [Description("Include closed opportunities in the results")]
            [DefaultValue(false)]
            public bool IncludeClosed { get; set; } = false;

            [CommandOption("--contains")]
            [Description("Use 'contains' search instead of 'starts with' (default is starts with)")]
            [DefaultValue(false)]
            public bool UseContains { get; set; } = false;
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(settings.AccountName))
            {
                AnsiConsole.MarkupLine("[red]Error: Account name is required[/]");
                return 1;
            }

            TokenProvider.clientSecret = CliContext.ClientSecret;
            TokenProvider.clientId = CliContext.ClientId;
            TokenProvider.tenantId = CliContext.TenantId;

            try
            {
                string? selectedAccountId = null;
                string? selectedAccountName = null;

                await AnsiConsole.Status()
                    .Spinner(Spinner.Known.Dots)
                    .StartAsync($"Searching for accounts matching '{settings.AccountName}'...", async ctx =>
                    {
                        DynamicsAccountsClient accountsClient = new DynamicsAccountsClient();
                        var accountsResult = await accountsClient.GetAccountsByName(
                            settings.AccountName, 
                            10, 
                            !settings.UseContains);

                        ctx.Status("Processing account search results...");

                        if (accountsResult.Value.Count == 0)
                        {
                            AnsiConsole.WriteLine();
                            AnsiConsole.MarkupLine($"[yellow]No accounts found matching '{settings.AccountName}'[/]");
                            return;
                        }
                        else if (accountsResult.Value.Count == 1)
                        {
                            selectedAccountId = accountsResult.Value[0].AccountId;
                            selectedAccountName = accountsResult.Value[0].Name;
                            AnsiConsole.WriteLine();
                            AnsiConsole.MarkupLine($"[green]Found account: {Markup.Escape(selectedAccountName ?? "N/A")}[/]");
                        }
                        else
                        {
                            ctx.Status("Multiple accounts found...");
                            AnsiConsole.WriteLine();
                            AnsiConsole.MarkupLine($"[yellow]Multiple accounts found matching '{settings.AccountName}'[/]");
                            AnsiConsole.WriteLine();

                            var accountChoices = accountsResult.Value
                                .Where(a => !string.IsNullOrWhiteSpace(a.AccountId))
                                .Select(a => new { Account = a, Display = $"{a.Name ?? "N/A"} (Owner: {a.OwnerName ?? "N/A"})" })
                                .ToList();

                            var selection = AnsiConsole.Prompt(
                                new SelectionPrompt<string>()
                                    .Title("[green]Select an account:[/]")
                                    .PageSize(10)
                                    .AddChoices(accountChoices.Select(c => c.Display))
                            );

                            var selectedAccount = accountChoices.First(c => c.Display == selection).Account;
                            selectedAccountId = selectedAccount.AccountId;
                            selectedAccountName = selectedAccount.Name;
                            AnsiConsole.WriteLine();
                        }
                    });

                if (string.IsNullOrWhiteSpace(selectedAccountId))
                {
                    return 1;
                }

                await AnsiConsole.Status()
                    .Spinner(Spinner.Known.Dots)
                    .StartAsync($"Retrieving opportunities for {selectedAccountName}...", async ctx =>
                    {
                        DynamicsOpportunitiesClient opportunitiesClient = new DynamicsOpportunitiesClient();
                        var result = await opportunitiesClient.GetOpportunitiesByAccount(selectedAccountId);

                        ctx.Status("Processing opportunities...");

                        var opportunities = settings.IncludeClosed
                            ? result.Value.OrderBy(o => o.StateCode).ToList()
                            : result.Value.Where(o => o.StateCode == 0).ToList();

                        if (settings.OutputJson)
                        {
                            AnsiConsole.WriteLine(JsonSerializer.Serialize(opportunities));
                        }
                        else
                        {
                            AnsiConsole.WriteLine();
                            var opportunityType = settings.IncludeClosed ? "Opportunities" : "Open opportunities";
                            AnsiConsole.MarkupLine($"[green]{opportunityType} for account '{Markup.Escape(selectedAccountName ?? "N/A")}':[/]");
                            AnsiConsole.WriteLine();

                            if (opportunities.Count == 0)
                            {
                                AnsiConsole.MarkupLine("[yellow]No opportunities found[/]");
                            }
                            else
                            {
                                var table = new Table();
                                table.AddColumn("[yellow]Number[/]");
                                table.AddColumn("[yellow]Auto number[/]");
                                table.AddColumn("[yellow]Name[/]");
                                table.AddColumn("[yellow]Created[/]");
                                table.AddColumn("[yellow]State[/]");
                                table.AddColumn("[yellow]Status[/]");
                                table.AddColumn("[yellow]Probability[/]");
                                table.AddColumn("[yellow]Owner[/]");
                                table.AddColumn("[yellow]Estimated Close Date[/]");

                                foreach (var opportunity in opportunities)
                                {
                                    table.AddRow(
                                        Markup.Escape(opportunity.OpportunityNumber ?? "N/A"),
                                        Markup.Escape(opportunity.AutoNumberOpportunity ?? "N/A"),
                                        Markup.Escape(opportunity.Name ?? "N/A"),
                                        Markup.Escape(DateTime.Parse(opportunity.CreatedOn).ToString("yyyy-MM-dd") ?? "N/A"),
                                        Markup.Escape(opportunity.StateCodeFormattedValue ?? "N/A"),
                                        Markup.Escape(opportunity.StatusCodeFormattedValue ?? "N/A"),
                                        Markup.Escape(opportunity.ProbabilityFormattedValue ?? "N/A"),
                                        Markup.Escape(opportunity.OwnerFormattedValue ?? "N/A"),
                                        Markup.Escape(opportunity.EstimatedCloseDate ?? "N/A")
                                    );
                                }

                                AnsiConsole.Write(table);
                            }
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

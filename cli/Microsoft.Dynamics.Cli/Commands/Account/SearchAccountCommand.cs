using Microsoft.Dynamics.Client;
using Partner.Center.Cli;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Dynamics.Cli.Commands.Account
{
    internal class SearchAccountCommand : AsyncCommand<SearchAccountCommand.Settings>
    {
        public SearchAccountCommand()
        {

        }

        public sealed class Settings : CommandSettings
        {
            [CommandOption("-n|--name <NAME>")]
            [Description("The name or partial name of the account to search for")]
            public string? Name { get; set; }

            [CommandOption("-t|--top <COUNT>")]
            [Description("Maximum number of results to return (default: 10)")]
            [DefaultValue(10)]
            public int Top { get; set; } = 10;

            [CommandOption("-c|--contains")]
            [Description("Use 'contains' search instead of 'starts with' (default is starts with)")]
            [DefaultValue(false)]
            public bool UseContains { get; set; } = false;
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(settings.Name))
            {
                AnsiConsole.MarkupLine("[red]Error: --name parameter is required[/]");
                return 1;
            }

            // Initialize TokenProvider
            TokenProvider.clientSecret = CliContext.ClientSecret;
            TokenProvider.clientId = CliContext.ClientId;
            TokenProvider.tenantId = CliContext.TenantId;

            string searchMode = settings.UseContains ? "containing" : "starting with";

            await AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .StartAsync($"Searching for accounts {searchMode} '{settings.Name}'...", async ctx =>
                {
                    DynamicsAccountsClient dynamicsAccountsClient = new DynamicsAccountsClient();
                    var accounts = await dynamicsAccountsClient.GetAccountsByName(settings.Name, settings.Top, !settings.UseContains);
                    
                    ctx.Status("Processing results...");
                    AnsiConsole.WriteLine();
                    AnsiConsole.MarkupLine($"[green]Search results for accounts {searchMode} '{settings.Name}':[/]");
                    AnsiConsole.WriteLine();

                    var table = new Table();
                    table.Border(TableBorder.Rounded);
                    table.AddColumn(new TableColumn("[yellow]Account Name[/]").LeftAligned());
                    table.AddColumn(new TableColumn("[yellow]Owner[/]").LeftAligned());
                    table.AddColumn(new TableColumn("[yellow]Account ID[/]").LeftAligned());

                    foreach (var account in accounts.Value)
                    {
                        table.AddRow(
                            account.Name ?? "[dim]N/A[/]",
                            account.OwnerName ?? "[dim]N/A[/]",
                            account.AccountId ?? "[dim]N/A[/]"
                        );
                    }

                    AnsiConsole.Write(table);
                    AnsiConsole.WriteLine();
                    AnsiConsole.MarkupLine($"[dim]Total results: {accounts.Value.Count}[/]");
                });

            return 0;
        }
    }
}
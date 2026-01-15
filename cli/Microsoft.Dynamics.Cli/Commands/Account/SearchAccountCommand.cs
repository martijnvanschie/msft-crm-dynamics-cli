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

            await AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .StartAsync($"Searching for accounts containing '{settings.Name}'...", async ctx =>
                {
                    DynamicsAccountsClient dynamicsAccountsClient = new DynamicsAccountsClient();
                    var accounts = await dynamicsAccountsClient.GetAccountsByNameContains(settings.Name, settings.Top);
                    
                    ctx.Status("Processing results...");
                    AnsiConsole.WriteLine();
                    AnsiConsole.MarkupLine($"[green]Search results for '{settings.Name}':[/]");
                    AnsiConsole.WriteLine(accounts);
                });

            return 0;
        }
    }
}


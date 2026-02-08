using Microsoft.Dynamics.Client.Model;
using Spectre.Console;
using Spectre.Console.Json;

namespace Microsoft.Dynamics.Cli.Utils
{
    internal class SpectreHelper
    {
        public static void RenderAccountsJson(List<AccountDTO> accounts)
        {
            if (accounts.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No accounts found[/]");
                return;
            }
            var json = System.Text.Json.JsonSerializer.Serialize(accounts, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

            var jsonText = new JsonText(json)
                    .MemberColor(Color.Yellow)
                    .StringColor(Color.White)
                    .NumberColor(Color.Aqua)
                    .BooleanColor(Color.Lime);

            AnsiConsole.Write(jsonText);
        }

        public static void RenderAccountsTable(List<AccountDTO> accounts)
        {
            if (accounts.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No accounts found[/]");
                return;
            }
            var table = new Table();
            table.Border(TableBorder.Rounded);
            table.AddColumn(new TableColumn("[yellow]Account Name[/]").LeftAligned());
            table.AddColumn(new TableColumn("[yellow]Owner[/]").LeftAligned());
            table.AddColumn(new TableColumn("[yellow]Territory[/]").LeftAligned());
            table.AddColumn(new TableColumn("[yellow]Relationship Type[/]").LeftAligned());
            table.AddColumn(new TableColumn("[yellow]Account ID[/]").LeftAligned());

            foreach (var account in accounts)
            {
                table.AddRow(
                    account.Name ?? "[dim]N/A[/]",
                    account.OwnerName ?? "[dim]N/A[/]",
                    account.TerritoryCodeFormattedValue ?? "[dim]N/A[/]",
                    account.RelationshipTypeFormattedValue ?? "[dim]N/A[/]",
                    account.AccountId ?? "[dim]N/A[/]"
                );
            }

            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[dim]Total results: {accounts.Count}[/]");
        }   

        public static void RenderOpportunitiesJson(List<OpportunityDTO> opportunities)
        {
            if (opportunities.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No opportunities found[/]");
                return;
            }
            var json = System.Text.Json.JsonSerializer.Serialize(opportunities, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

            var jsonText = new JsonText(json)
                    .MemberColor(Color.Yellow)
                    .StringColor(Color.White)
                    .NumberColor(Color.Aqua)
                    .BooleanColor(Color.Lime);

            AnsiConsole.Write(jsonText);
        }

        public static void RenderOpportunitiesTable(List<OpportunityDTO> opportunities)
        {
            if (opportunities.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No opportunities found[/]");
                return;
            }

            var table = new Table();

            table.AddColumn("[yellow]Number[/]");
            table.AddColumn("[yellow]Auto number[/]");
            table.AddColumn("[yellow]Parent Account[/]");
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
                    Markup.Escape(opportunity.ParentAccountIdFormattedValue ?? "N/A"),
                    Markup.Escape(opportunity.Name ?? "N/A"),
                    Markup.Escape(
                        !string.IsNullOrEmpty(opportunity.CreatedOn)
                            ? DateTime.Parse(opportunity.CreatedOn).ToString("yyyy-MM-dd")
                            : "N/A"
                    ),
                    Markup.Escape(opportunity.StateCodeFormattedValue ?? "N/A"),
                    Markup.Escape(opportunity.StatusCodeFormattedValue ?? "N/A"),
                    Markup.Escape(opportunity.ProbabilityFormattedValue ?? "N/A"),
                    Markup.Escape(opportunity.OwnerFormattedValue ?? "N/A"),
                    Markup.Escape(opportunity.EstimatedCloseDate ?? "N/A")
                );
            }

            AnsiConsole.Write(table);
        }

        public static void RenderOpportunitiesListTable(List<OpportunityDTO> opportunities)
        {
            if (opportunities.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No opportunities found[/]");
                return;
            }

            var table = new Table();

            //table.AddColumn("[yellow]Number[/]");
            //table.AddColumn("[yellow]Auto number[/]");
            table.AddColumn("[yellow]Parent Account[/]");
            table.AddColumn("[yellow]Name[/]");
            table.AddColumn("[yellow]Created[/]");
            table.AddColumn("[yellow]State[/]");
            table.AddColumn("[yellow]Status[/]");
            table.AddColumn("[yellow]Probability[/]");
            table.AddColumn("[yellow]Owner[/]");
            //table.AddColumn("[yellow]Estimated Close Date[/]");

            foreach (var opportunity in opportunities)
            {
                table.AddRow(
                    //Markup.Escape(opportunity.OpportunityNumber ?? "N/A"),
                    //Markup.Escape(opportunity.AutoNumberOpportunity ?? "N/A"),
                    Markup.Escape(opportunity.ParentAccountIdFormattedValue ?? "N/A"),
                    Markup.Escape(opportunity.Name ?? "N/A"),
                    Markup.Escape(
                        !string.IsNullOrEmpty(opportunity.CreatedOn)
                            ? DateTime.Parse(opportunity.CreatedOn).ToString("yyyy-MM-dd")
                            : "N/A"
                    ),
                    Markup.Escape(opportunity.StateCodeFormattedValue ?? "N/A"),
                    Markup.Escape(opportunity.StatusCodeFormattedValue ?? "N/A"),
                    Markup.Escape(opportunity.ProbabilityFormattedValue ?? "N/A"),
                    Markup.Escape(opportunity.OwnerFormattedValue ?? "N/A")
                //Markup.Escape(opportunity.EstimatedCloseDate ?? "N/A")
                );
            }

            AnsiConsole.Write(table);
        }
        //JsonText
    }
}

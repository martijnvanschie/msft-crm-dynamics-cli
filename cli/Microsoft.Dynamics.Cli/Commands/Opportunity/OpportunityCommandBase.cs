using Microsoft.Dynamics.Client.Model;
using Spectre.Console;

namespace Microsoft.Dynamics.Cli.Commands.Opportunity
{
    internal static class OpportunityTableRenderer
    {
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
    }
}

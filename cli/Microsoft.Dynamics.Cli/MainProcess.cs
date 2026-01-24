using Microsoft.Dynamics.Client;
using Microsoft.Dynamics.Core.Logging;
using Microsoft.Extensions.Logging;
using Partner.Center.Cli;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Microsoft.Dynamics.Cli
{
    internal class MainProcess
    {
        private static ILogger<MainProcess> _logger;

        public MainProcess(ILogger<MainProcess> logger)
        {
            _logger = logger;
        }

        internal async Task<int> RunAsync(string[] args)
        {
            _logger.LogInformation("Current process folder: {folder}", Environment.CurrentDirectory);
            _logger.LogInformation("Current thread id: {threadId}", Environment.ProcessId);

            var app = new CommandApp();

            app.Configure(config =>
            {
                config.SetApplicationName("mdcli");
                config.SetApplicationVersion("1.0");

#if DEBUG
                config.PropagateExceptions();
#endif

                // Configure account commands
                config.AddBranch("account", account =>
                {
                    account.AddCommand<Commands.Account.SearchAccountCommand>("search")
                        .WithDescription("Search for accounts by name")
                        .WithExample(new[] { "account", "search", "--name", "Contoso" })
                        .WithExample(new[] { "account", "search", "-n", "Microsoft", "--top", "5" })
                        .WithExample(new[] { "account", "search", "-n", "Corp", "--contains" });
                });

                config.ValidateExamples();

                config.SetExceptionHandler((ex, resolver) =>
                {
                    AnsiConsole.MarkupInterpolated($"[red]{ex.Message}[/]");
                    return 1;
                });

            });

            try
            {
                return await app.RunAsync(args);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupInterpolated($"[red]{ex.Message}[/]");
                return -1;
            }

            //HttpClient _http = new HttpClient();
            //_http.BaseAddress = new Uri("https://api.partnercenter.microsoft.com/v1/");
            //_http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", pctoken.AccessToken);

            //var response = await _http.GetAsync("customers");
            //try
            //{
            //	response.EnsureSuccessStatusCode();
            //}
            //catch (Exception ex)
            //{
            //	Console.WriteLine(ex);
            //}
            //var response2 = await response.Content.ReadAsStringAsync();

            ////// Dynamics 365 Sales API call
            //// Get opportunity by ID
            ///
            TokenProvider.clientSecret = CliContext.ClientSecret;
            TokenProvider.clientId = CliContext.ClientId;
            TokenProvider.tenantId = CliContext.TenantId;

            string opportunityId = "e59cc412-8adc-483d-8d96-47f4bd0fe4cf"; // Replace with actual opportunity ID

            DynamicsOpportunitiesClient dynamicsOpportunitiesClient = new DynamicsOpportunitiesClient();
            var opportunities = await dynamicsOpportunitiesClient.GetOpportunitiesWithFields(new string[] { "name", "estimatedvalue", "statuscode" }, 5);
            Console.WriteLine(opportunities);

            var opportunityById = await dynamicsOpportunitiesClient.GetOpportunity(opportunityId);
            Console.WriteLine(opportunityById);

            DynamicsAccountsClient dynamicsAccountsClient = new DynamicsAccountsClient();
            var accounts = await dynamicsAccountsClient.GetAccountsByName("Zeeman", 5);
            Console.WriteLine(accounts);

            return 0;
        }
    }
}
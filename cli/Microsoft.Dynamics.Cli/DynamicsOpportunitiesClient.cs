using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Partner.Center.Cli
{
    internal class DynamicsOpportunitiesClient
    {
        private const string DynamicsUrl = "https://macaw.crm4.dynamics.com";
        private readonly HttpClient _httpClient;

        internal DynamicsOpportunitiesClient()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri($"{DynamicsUrl}/api/data/v9.2/");
        }

        private async Task InitializeAuthenticationAsync()
        {
            var token = await TokenProvider.GetDynamicsUserToken();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            _httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            _httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
            _httpClient.DefaultRequestHeaders.Add("Prefer", "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Get a specific opportunity by ID
        /// </summary>
        /// <param name="opportunityId">The GUID of the opportunity</param>
        /// <returns>Formatted JSON string with opportunity details</returns>
        /// <example>
        /// // Get specific opportunity
        /// var opportunity = await client.GetOpportunity("opportunity-guid-here");
        /// Console.WriteLine(opportunity);
        /// </example>
        internal async Task<string> GetOpportunity(string opportunityId)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"opportunities({opportunityId})");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        /// <summary>
        /// Get all opportunities with optional limit
        /// </summary>
        /// <param name="top">Maximum number of opportunities to return</param>
        /// <returns>Formatted JSON string with opportunities list</returns>
        internal async Task<string> GetOpportunities(int top = 10)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"opportunities?$top={top}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        /// <summary>
        /// Get opportunities with specific fields
        /// </summary>
        /// <param name="fields">Array of field names to retrieve</param>
        /// <param name="top">Maximum number of opportunities to return</param>
        /// <returns>Formatted JSON string with selected fields</returns>
        /// <example>
        /// // Get opportunities with specific fields
        /// var opportunities = await client.GetOpportunitiesWithFields(new[] { "name", "estimatedvalue", "actualvalue", "closeprobability" }, 10);
        /// Console.WriteLine(opportunities);
        /// </example>
        internal async Task<string> GetOpportunitiesWithFields(string[] fields, int top = 10)
        {
            await InitializeAuthenticationAsync();
            var select = string.Join(",", fields);
            var response = await _httpClient.GetAsync($"opportunities?$select={select}&$top={top}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        /// <summary>
        /// Filter opportunities by criteria
        /// </summary>
        /// <param name="filter">OData filter expression</param>
        /// <param name="top">Maximum number of opportunities to return</param>
        /// <returns>Formatted JSON string with filtered opportunities</returns>
        /// <example>
        /// // Filter opportunities by estimated value
        /// var filtered = await client.GetOpportunitiesByFilter("estimatedvalue gt 100000", 10);
        /// Console.WriteLine(filtered);
        /// </example>
        internal async Task<string> GetOpportunitiesByFilter(string filter, int top = 10)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"opportunities?$filter={filter}&$top={top}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        /// <summary>
        /// Get opportunity with expanded related data
        /// </summary>
        /// <param name="opportunityId">The GUID of the opportunity</param>
        /// <param name="expand">OData expand expression</param>
        /// <returns>Formatted JSON string with opportunity and related data</returns>
        /// <example>
        /// // Get opportunity with related account
        /// var opportunity = await client.GetOpportunityWithRelatedData("opportunity-guid-here", "customerid_account($select=name,accountnumber)");
        /// Console.WriteLine(opportunity);
        /// </example>
        internal async Task<string> GetOpportunityWithRelatedData(string opportunityId, string expand)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"opportunities({opportunityId})?$expand={expand}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        /// <summary>
        /// Get all opportunities for a specific account
        /// </summary>
        /// <param name="accountId">The GUID of the account</param>
        /// <returns>Formatted JSON string with opportunities for the account</returns>
        /// <example>
        /// // Get opportunities by account
        /// var accountOpportunities = await client.GetOpportunitiesByAccount("account-guid-here");
        /// Console.WriteLine(accountOpportunities);
        /// </example>
        internal async Task<string> GetOpportunitiesByAccount(string accountId)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"accounts({accountId})/opportunity_customer_accounts");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        /// <summary>
        /// Get all opportunities for a specific contact
        /// </summary>
        /// <param name="contactId">The GUID of the contact</param>
        /// <returns>Formatted JSON string with opportunities for the contact</returns>
        internal async Task<string> GetOpportunitiesByContact(string contactId)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"contacts({contactId})/opportunity_customer_contacts");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        /// <summary>
        /// Get opportunities by sales stage
        /// </summary>
        /// <param name="salesStageCode">The sales stage code (e.g., 0=Qualify, 1=Develop, 2=Propose, 3=Close)</param>
        /// <param name="top">Maximum number of opportunities to return</param>
        /// <returns>Formatted JSON string with opportunities in the specified stage</returns>
        /// <example>
        /// // Get opportunities in Propose stage
        /// var proposalOpportunities = await client.GetOpportunitiesBySalesStage(2, 10);
        /// Console.WriteLine(proposalOpportunities);
        /// </example>
        internal async Task<string> GetOpportunitiesBySalesStage(int salesStageCode, int top = 10)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"opportunities?$filter=salesstage eq {salesStageCode}&$top={top}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        /// <summary>
        /// Get open opportunities (statecode = 0)
        /// </summary>
        /// <param name="top">Maximum number of opportunities to return</param>
        /// <returns>Formatted JSON string with open opportunities</returns>
        internal async Task<string> GetOpenOpportunities(int top = 10)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"opportunities?$filter=statecode eq 0&$top={top}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        /// <summary>
        /// Get won opportunities (statecode = 1 and statuscode = 3)
        /// </summary>
        /// <param name="top">Maximum number of opportunities to return</param>
        /// <returns>Formatted JSON string with won opportunities</returns>
        internal async Task<string> GetWonOpportunities(int top = 10)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"opportunities?$filter=statecode eq 1 and statuscode eq 3&$top={top}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        private static string FormatJson(string json)
        {
            var jsonDocument = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(jsonDocument, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
    }
}

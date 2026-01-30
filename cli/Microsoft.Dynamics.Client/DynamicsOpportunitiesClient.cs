using Microsoft.Dynamics.Client.Model;
using Microsoft.Dynamics.Core.Logging;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Microsoft.Dynamics.Client
{
    public class DynamicsOpportunitiesClient : DynamicsClientBase
    {
        protected readonly static ILogger<DynamicsOpportunitiesClient> _logger = LoggerManager.GetLogger<DynamicsOpportunitiesClient>();

        private const int DEFAULT_TOP_VALUE = 20;

        public DynamicsOpportunitiesClient() : base()
        {
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
        public async Task<string> GetOpportunity(string opportunityId)
        {
            _logger.LogDebug("Getting opportunity with ID: {OpportunityId}", opportunityId);
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
        public async Task<string> GetOpportunities(int top = DEFAULT_TOP_VALUE)
        {
            _logger.LogDebug("Getting opportunities with top value: {Top}", top);
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
        public async Task<string> GetOpportunitiesWithFields(string[] fields, int top = DEFAULT_TOP_VALUE)
        {
            _logger.LogDebug("Getting opportunities with fields: {Fields} and top value: {Top}", string.Join(",", fields), top);
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
        public async Task<string> GetOpportunitiesByFilter(string filter, int top = DEFAULT_TOP_VALUE)
        {
            _logger.LogDebug("Getting opportunities with filter: {Filter} and top value: {Top}", filter, top);
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
        public async Task<string> GetOpportunityWithRelatedData(string opportunityId, string expand)
        {
            _logger.LogDebug("Getting opportunity with ID: {OpportunityId} and related data: {Expand}", opportunityId, expand);
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
        /// <param name="top">Maximum number of opportunities to return</param>
        /// <param name="includeClosed">If true, includes closed opportunities; otherwise only open opportunities (statecode = 0)</param>
        /// <returns>OpportunitiesResponseDTO containing opportunities for the account</returns>
        /// <example>
        /// // Get opportunities by account
        /// var accountOpportunities = await client.GetOpportunitiesByAccount("account-guid-here", 20, false);
        /// Console.WriteLine(accountOpportunities);
        /// </example>
        public async Task<OpportunitiesResponseDTO> GetOpportunitiesByAccount(string accountId, int top = DEFAULT_TOP_VALUE, bool includeClosed = false)
        {
            _logger.LogDebug("Getting opportunities for account with ID: {AccountId}, top value: {Top}, and includeClosed: {IncludeClosed}", accountId, top, includeClosed);
            await InitializeAuthenticationAsync();
            
            string filter = includeClosed ? "" : "$filter=statecode eq 0&";
            var response = await _httpClient.GetAsync($"accounts({accountId})/opportunity_customer_accounts?{filter}$top={top}&$orderby=estimatedclosedate desc");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<OpportunitiesResponseDTO>(json);
        }

        /// <summary>
        /// Get all opportunities for a specific contact
        /// </summary>
        /// <param name="contactId">The GUID of the contact</param>
        /// <param name="top">Maximum number of opportunities to return</param>
        /// <returns>Formatted JSON string with opportunities for the contact</returns>
        public async Task<string> GetOpportunitiesByContact(string contactId, int top = DEFAULT_TOP_VALUE)
        {
            _logger.LogDebug("Getting opportunities for contact with ID: {ContactId} and top value: {Top}", contactId, top);
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"contacts({contactId})/opportunity_customer_contacts?$top={top}");
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
        public async Task<string> GetOpportunitiesBySalesStage(int salesStageCode, int top = DEFAULT_TOP_VALUE)
        {
            _logger.LogDebug("Getting opportunities with sales stage code: {SalesStageCode} and top value: {Top}", salesStageCode, top);
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
        public async Task<string> GetOpenOpportunities(int top = DEFAULT_TOP_VALUE)
        {
            _logger.LogDebug("Getting open opportunities with top value: {Top}", top);
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
        public async Task<string> GetWonOpportunities(int top = DEFAULT_TOP_VALUE)
        {
            _logger.LogDebug("Getting won opportunities with top value: {Top}", top);
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"opportunities?$filter=statecode eq 1 and statuscode eq 3&$top={top}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        /// <summary>
        /// Search for opportunities by name
        /// </summary>
        /// <param name="searchString">The name or partial name to search for</param>
        /// <param name="top">Maximum number of opportunities to return</param>
        /// <param name="useStartsWith">If true, uses 'startswith' filter; otherwise uses 'contains' filter</param>
        /// <param name="includeClosed">If true, includes closed opportunities; otherwise only open opportunities (statecode = 0)</param>
        /// <returns>OpportunitiesResponseDTO containing the search results</returns>
        /// <example>
        /// // Search for opportunities starting with a name
        /// var opportunities = await client.GetOpportunitiesByName("Microsoft", 10, true, false);
        /// Console.WriteLine(opportunities);
        /// </example>
        public async Task<OpportunitiesResponseDTO> GetOpportunitiesByName(string searchString, int top = DEFAULT_TOP_VALUE, bool useStartsWith = true, bool includeClosed = false)
        {
            _logger.LogDebug("Getting opportunities by name with search string: {SearchString}, top value: {Top}, useStartsWith: {UseStartsWith}, and includeClosed: {IncludeClosed}", searchString, top, useStartsWith, includeClosed);
            await InitializeAuthenticationAsync();

            string nameFilter = useStartsWith 
                ? $"startswith(name,'{searchString}')" 
                : $"contains(name,'{searchString}')";

            string filter = includeClosed 
                ? nameFilter 
                : $"{nameFilter} and statecode eq 0";

            var response = await _httpClient.GetAsync($"opportunities?$filter={filter}&$top={top}&$orderby=createdon desc");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<OpportunitiesResponseDTO>(json);
        }
    }
}

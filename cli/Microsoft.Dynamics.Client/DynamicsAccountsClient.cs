using Microsoft.Dynamics.Client.Model;
using Microsoft.Dynamics.Core.Logging;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Microsoft.Dynamics.Client
{
    public class DynamicsAccountsClient : DynamicsClientBase
    {
        protected readonly static new ILogger<DynamicsAccountsClient> _logger = LoggerManager.GetLogger<DynamicsAccountsClient>();

        private const int DEFAULT_TOP_VALUE = 20;

        public DynamicsAccountsClient() : base()
        {
        }

        public async Task<string> GetAccount(string accountId)
        {
            _logger.LogDebug("Getting account with ID: {AccountId}", accountId);
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"accounts({accountId})");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        public async Task<string> GetAccounts(int top = DEFAULT_TOP_VALUE)
        {
            _logger.LogDebug("Getting accounts with top value: {Top}", top);
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"accounts?$top={top}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        public async Task<string> GetAccountsWithFields(string[] fields, int top = DEFAULT_TOP_VALUE)
        {
            _logger.LogDebug("Getting accounts with fields: {Fields} and top value: {Top}", string.Join(",", fields), top);
            await InitializeAuthenticationAsync();
            var select = string.Join(",", fields);
            var response = await _httpClient.GetAsync($"accounts?$select={select}&$top={top}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        public async Task<string> GetAccountsByFilter(string filter, int top = DEFAULT_TOP_VALUE)
        {
            _logger.LogDebug("Getting accounts with filter: {Filter} and top value: {Top}", filter, top);
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"accounts?$filter={filter}&$top={top}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        public async Task<string> GetAccountWithRelatedData(string accountId, string expand)
        {
            _logger.LogDebug("Getting account with ID: {AccountId} and related data: {Expand}", accountId, expand);
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"accounts({accountId})?$expand={expand}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        public async Task<string> GetOpportunitiesForAccount(string accountId, int top = DEFAULT_TOP_VALUE)
        {
            _logger.LogDebug("Getting opportunities for account with ID: {AccountId} and top value: {Top}", accountId, top);
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"accounts({accountId})/opportunity_customer_accounts?$top={top}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        public async Task<string> GetOpportunity(string opportunityId)
        {
            _logger.LogDebug("Getting opportunity with ID: {OpportunityId}", opportunityId);
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"opportunities({opportunityId})");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        public async Task<AccountsResponseDTO> GetAccountsByName(string searchString, int top = DEFAULT_TOP_VALUE, bool useStartsWith = true)
        {
            _logger.LogDebug("Getting accounts by name with search string: {SearchString}, top value: {Top}, and useStartsWith: {UseStartsWith}", searchString, top, useStartsWith);
            await InitializeAuthenticationAsync();

            string[] fields = new string[] { "name", "_ownerid_value" };
            string select = string.Join(",", fields);

            string filter = useStartsWith
                ? $"startswith(name,'{searchString}')"
                : $"contains(name,'{searchString}')";

            var response = await _httpClient.GetAsync($"accounts?$filter={filter}&$top={top}&$select={select}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AccountsResponseDTO>(json);
            if (result == null)
            {
                throw new InvalidOperationException("Failed to deserialize AccountsResponseDTO from response JSON.");
            }
            return result;
        }

    }
}

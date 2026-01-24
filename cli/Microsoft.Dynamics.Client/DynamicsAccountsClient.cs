using Microsoft.Dynamics.Client.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.Dynamics.Client
{
    public class DynamicsAccountsClient : DynamicsClientBase
    {
        public DynamicsAccountsClient() : base()
        {
        }

        public async Task<string> GetAccount(string accountId)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"accounts({accountId})");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        public async Task<string> GetAccounts(int top = 10)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"accounts?$top={top}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        public async Task<string> GetAccountsWithFields(string[] fields, int top = 10)
        {
            await InitializeAuthenticationAsync();
            var select = string.Join(",", fields);
            var response = await _httpClient.GetAsync($"accounts?$select={select}&$top={top}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        public async Task<string> GetAccountsByFilter(string filter, int top = 10)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"accounts?$filter={filter}&$top={top}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        public async Task<string> GetAccountWithRelatedData(string accountId, string expand)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"accounts({accountId})?$expand={expand}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        public async Task<string> GetOpportunitiesForAccount(string accountId)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"accounts({accountId})/opportunity_customer_accounts");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        public async Task<string> GetOpportunity(string opportunityId)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"opportunities({opportunityId})");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        public async Task<AccountSearchResultDTO> GetAccountsByName(string searchString, int top = 10, bool useStartsWith = true)
        {
            await InitializeAuthenticationAsync();

            string[] fields = new string[] {"name", "_ownerid_value" };
            string select = string.Join(",", fields);

            string filter = useStartsWith 
                ? $"startswith(name,'{searchString}')" 
                : $"contains(name,'{searchString}')";

            var response = await _httpClient.GetAsync($"accounts?$filter={filter}&$top={top}&$select={select}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<AccountSearchResultDTO>(json);
            //return FormatJson(json);
        }

    }
}

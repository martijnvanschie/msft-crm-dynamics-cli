using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Partner.Center.Cli
{
    internal class DynamicsAccountsClient : DynamicsClientBase
    {
        internal DynamicsAccountsClient() : base()
        {
        }

        internal async Task<string> GetAccount(string accountId)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"accounts({accountId})");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        internal async Task<string> GetAccounts(int top = 10)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"accounts?$top={top}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        internal async Task<string> GetAccountsWithFields(string[] fields, int top = 10)
        {
            await InitializeAuthenticationAsync();
            var select = string.Join(",", fields);
            var response = await _httpClient.GetAsync($"accounts?$select={select}&$top={top}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        internal async Task<string> GetAccountsByFilter(string filter, int top = 10)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"accounts?$filter={filter}&$top={top}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        internal async Task<string> GetAccountWithRelatedData(string accountId, string expand)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"accounts({accountId})?$expand={expand}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        internal async Task<string> GetOpportunitiesForAccount(string accountId)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"accounts({accountId})/opportunity_customer_accounts");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        internal async Task<string> GetOpportunity(string opportunityId)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"opportunities({opportunityId})");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

    }
}

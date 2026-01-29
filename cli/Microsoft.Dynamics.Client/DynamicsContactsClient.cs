using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.Dynamics.Client
{
    internal class DynamicsContactsClient : DynamicsClientBase
    {
        private const int DEFAULT_TOP_VALUE = 20;

        internal DynamicsContactsClient() : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        /// <example>
        /// // Get specific contact
        /// var contact = await client.GetContact("contact-guid-here");
        /// Console.WriteLine(contact);
        /// </example>
        internal async Task<string> GetContact(string contactId)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"contacts({contactId})");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        internal async Task<string> GetContacts(int top = DEFAULT_TOP_VALUE)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"contacts?$top={top}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        /// <example>
        /// // Get contacts with specific fields
        /// var contacts = await client.GetContactsWithFields(new[] { "fullname", "emailaddress1", "telephone1", "jobtitle" }, 10);
        /// Console.WriteLine(contacts);
        /// </example>
        internal async Task<string> GetContactsWithFields(string[] fields, int top = DEFAULT_TOP_VALUE)
        {
            await InitializeAuthenticationAsync();
            var select = string.Join(",", fields);
            var response = await _httpClient.GetAsync($"contacts?$select={select}&$top={top}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        /// <example>
        /// // Filter contacts by name
        /// var filtered = await client.GetContactsByFilter("contains(fullname,'Smith')", 10);
        /// Console.WriteLine(filtered);
        /// </example>
        internal async Task<string> GetContactsByFilter(string filter, int top = DEFAULT_TOP_VALUE)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"contacts?$filter={filter}&$top={top}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        internal async Task<string> GetContactWithRelatedData(string contactId, string expand)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"contacts({contactId})?$expand={expand}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        internal async Task<string> GetContactsByAccount(string accountId, int top = DEFAULT_TOP_VALUE)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"accounts({accountId})/contact_customer_accounts?$top={top}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        internal async Task<string> GetOpportunitiesForContact(string contactId, int top = DEFAULT_TOP_VALUE)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"contacts({contactId})/opportunity_customer_contacts?$top={top}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.Dynamics.Client
{
    internal class DynamicsLeadsClient : DynamicsClientBase
    {
        internal DynamicsLeadsClient() : base()
        {
        }

        /// <summary>
        /// Get a specific lead by ID
        /// </summary>
        /// <param name="leadId">The GUID of the lead</param>
        /// <returns>Formatted JSON string with lead details</returns>
        /// <example>
        /// // Get specific lead
        /// var lead = await client.GetLead("lead-guid-here");
        /// Console.WriteLine(lead);
        /// </example>
        internal async Task<string> GetLead(string leadId)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"leads({leadId})");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        /// <summary>
        /// Get all leads with optional limit
        /// </summary>
        /// <param name="top">Maximum number of leads to return</param>
        /// <returns>Formatted JSON string with leads list</returns>
        internal async Task<string> GetLeads(int top = 10)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"leads?$top={top}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        /// <summary>
        /// Get leads with specific fields
        /// </summary>
        /// <param name="fields">Array of field names to retrieve</param>
        /// <param name="top">Maximum number of leads to return</param>
        /// <returns>Formatted JSON string with selected fields</returns>
        /// <example>
        /// // Get leads with specific fields
        /// var leads = await client.GetLeadsWithFields(new[] { "fullname", "companyname", "emailaddress1", "telephone1" }, 10);
        /// Console.WriteLine(leads);
        /// </example>
        internal async Task<string> GetLeadsWithFields(string[] fields, int top = 10)
        {
            await InitializeAuthenticationAsync();
            var select = string.Join(",", fields);
            var response = await _httpClient.GetAsync($"leads?$select={select}&$top={top}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        /// <summary>
        /// Filter leads by criteria
        /// </summary>
        /// <param name="filter">OData filter expression</param>
        /// <param name="top">Maximum number of leads to return</param>
        /// <returns>Formatted JSON string with filtered leads</returns>
        /// <example>
        /// // Filter leads by company name
        /// var filtered = await client.GetLeadsByFilter("contains(companyname,'Microsoft')", 10);
        /// Console.WriteLine(filtered);
        /// </example>
        internal async Task<string> GetLeadsByFilter(string filter, int top = 10)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"leads?$filter={filter}&$top={top}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        /// <summary>
        /// Get lead with expanded related data
        /// </summary>
        /// <param name="leadId">The GUID of the lead</param>
        /// <param name="expand">OData expand expression</param>
        /// <returns>Formatted JSON string with lead and related data</returns>
        /// <example>
        /// // Get lead with related contact
        /// var lead = await client.GetLeadWithRelatedData("lead-guid-here", "parentcontactid($select=fullname,emailaddress1)");
        /// Console.WriteLine(lead);
        /// </example>
        internal async Task<string> GetLeadWithRelatedData(string leadId, string expand)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"leads({leadId})?$expand={expand}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        /// <summary>
        /// Get leads by qualification status
        /// </summary>
        /// <param name="qualifyingCode">The qualification status code (e.g., 1=New, 2=Contacted, 3=Qualified, 4=Lost, 5=Cannot Contact, 6=No Longer Interested, 7=Canceled)</param>
        /// <param name="top">Maximum number of leads to return</param>
        /// <returns>Formatted JSON string with leads in the specified status</returns>
        /// <example>
        /// // Get qualified leads
        /// var qualifiedLeads = await client.GetLeadsByQualificationStatus(3, 10);
        /// Console.WriteLine(qualifiedLeads);
        /// </example>
        internal async Task<string> GetLeadsByQualificationStatus(int qualifyingCode, int top = 10)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"leads?$filter=leadqualitycode eq {qualifyingCode}&$top={top}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        /// <summary>
        /// Get open leads (statecode = 0)
        /// </summary>
        /// <param name="top">Maximum number of leads to return</param>
        /// <returns>Formatted JSON string with open leads</returns>
        internal async Task<string> GetOpenLeads(int top = 10)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"leads?$filter=statecode eq 0&$top={top}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        /// <summary>
        /// Get qualified leads (statecode = 1 and statuscode = 3)
        /// </summary>
        /// <param name="top">Maximum number of leads to return</param>
        /// <returns>Formatted JSON string with qualified leads</returns>
        internal async Task<string> GetQualifiedLeads(int top = 10)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"leads?$filter=statecode eq 1 and statuscode eq 3&$top={top}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        /// <summary>
        /// Get leads by rating
        /// </summary>
        /// <param name="ratingCode">The rating code (e.g., 1=Hot, 2=Warm, 3=Cold)</param>
        /// <param name="top">Maximum number of leads to return</param>
        /// <returns>Formatted JSON string with leads of the specified rating</returns>
        /// <example>
        /// // Get hot leads
        /// var hotLeads = await client.GetLeadsByRating(1, 10);
        /// Console.WriteLine(hotLeads);
        /// </example>
        internal async Task<string> GetLeadsByRating(int ratingCode, int top = 10)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"leads?$filter=leadqualitycode eq {ratingCode}&$top={top}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

        /// <summary>
        /// Get leads by source
        /// </summary>
        /// <param name="leadsourceCode">The lead source code (e.g., 1=Advertisement, 2=Employee Referral, 3=External Referral, 4=Partner, 5=Public Relations, 6=Seminar, 7=Trade Show, 8=Web, 9=Word of Mouth, 10=Other)</param>
        /// <param name="top">Maximum number of leads to return</param>
        /// <returns>Formatted JSON string with leads from the specified source</returns>
        /// <example>
        /// // Get leads from web
        /// var webLeads = await client.GetLeadsBySource(8, 10);
        /// Console.WriteLine(webLeads);
        /// </example>
        internal async Task<string> GetLeadsBySource(int leadsourceCode, int top = 10)
        {
            await InitializeAuthenticationAsync();
            var response = await _httpClient.GetAsync($"leads?$filter=leadsourcecode eq {leadsourceCode}&$top={top}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return FormatJson(json);
        }

    }
}

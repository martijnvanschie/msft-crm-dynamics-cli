using Microsoft.Identity.Client;
using Partner.Center.Cli;
using System.Net.Http.Headers;
using static System.Net.WebRequestMethods;

// See https://aka.ms/new-console-template for more information
var pctoken = await TokenProvider.GetPartnerCenterClientToken();

HttpClient _http = new HttpClient();
_http.BaseAddress = new Uri("https://api.partnercenter.microsoft.com/v1/");
_http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", pctoken.AccessToken);

var response = await _http.GetAsync("customers");
try
{
	response.EnsureSuccessStatusCode();
}
catch (Exception ex)
{
	Console.WriteLine(ex);
}
var response2 = await response.Content.ReadAsStringAsync();

//// Dynamics 365 Sales API call
string dynamicsUrl = "https://macaw.crm4.dynamics.com";

// Example of delegated auth (requires user interaction)
var token = await TokenProvider.GetDynamicsUserToken();

HttpClient dynamicsHttp = new HttpClient();
dynamicsHttp.BaseAddress = new Uri($"{dynamicsUrl}/api/data/v9.2/");
dynamicsHttp.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
dynamicsHttp.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
dynamicsHttp.DefaultRequestHeaders.Add("OData-Version", "4.0");
dynamicsHttp.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

// Get opportunity by ID
string opportunityId = "e59cc412-8adc-483d-8d96-47f4bd0fe4cf"; // Replace with actual opportunity ID
var opportunityResponse = await dynamicsHttp.GetAsync($"opportunities({opportunityId})");
try
{
    opportunityResponse.EnsureSuccessStatusCode();
    var opportunityJson = await opportunityResponse.Content.ReadAsStringAsync();
    
    // Format JSON with indentation
    var jsonDocument = System.Text.Json.JsonDocument.Parse(opportunityJson);
    var formattedJson = System.Text.Json.JsonSerializer.Serialize(jsonDocument, new System.Text.Json.JsonSerializerOptions 
    { 
        WriteIndented = true 
    });
    
    Console.WriteLine("Opportunity Details:");
    Console.WriteLine(formattedJson);
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}

DynamicsOpportunitiesClient dynamicsOpportunitiesClient = new DynamicsOpportunitiesClient();
var opportunities = await dynamicsOpportunitiesClient.GetOpportunitiesWithFields(new string[] { "name", "estimatedvalue", "statuscode" }, 5);
Console.WriteLine(opportunities);

//DynamicsAccountsClient dynamicsClient = new DynamicsAccountsClient();
//var accounts = await dynamicsClient.GetAccounts();
//Console.WriteLine(accounts);

return 0;
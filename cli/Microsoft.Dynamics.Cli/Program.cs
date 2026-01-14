using Microsoft.Identity.Client;
using Partner.Center.Cli;
using System.Net.Http.Headers;
using Microsoft.Dynamics.Client;
using static System.Net.WebRequestMethods;

//// See https://aka.ms/new-console-template for more information
//var pctoken = await TokenProvider.GetPartnerCenterClientToken();

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

//DynamicsAccountsClient dynamicsClient = new DynamicsAccountsClient();
//var accounts = await dynamicsClient.GetAccounts();
//Console.WriteLine(accounts);

return 0;
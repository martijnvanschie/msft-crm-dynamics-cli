using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.Dynamics.Client
{
    public abstract class DynamicsClientBase
    {
        private const string _dynamicsUrl = "https://macaw.crm4.dynamics.com";
        protected readonly HttpClient _httpClient;

        protected DynamicsClientBase()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri($"{_dynamicsUrl}/api/data/v9.2/");
        }

        protected async Task InitializeAuthenticationAsync()
        {
            var token = await TokenProvider.GetDynamicsUserToken();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            
            if (!_httpClient.DefaultRequestHeaders.Contains("OData-MaxVersion"))
            {
                _httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                _httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
            }
            
            if (_httpClient.DefaultRequestHeaders.Accept.Count == 0)
            {
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }

            if (!_httpClient.DefaultRequestHeaders.Contains("Prefer"))
            {
                _httpClient.DefaultRequestHeaders.Add("Prefer", "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"");
            }
        }

        protected static string FormatJson(string json)
        {
            var jsonDocument = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(jsonDocument, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
    }
}

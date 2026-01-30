using Microsoft.Dynamics.Core.Configuration;
using Microsoft.Dynamics.Core.Logging;
using Microsoft.Extensions.Logging;
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
        protected private readonly static ILogger<DynamicsClientBase> _logger = LoggerManager.GetLogger<DynamicsClientBase>();

        private static readonly Lazy<HttpClient> _lazyHttpClient = new Lazy<HttpClient>(() =>
        {
            try
            {
                var config = ConfigManager.GetConfiguration();
                var dynamicsUrl = config.Settings.DynamicsUrl
                    ?? throw new InvalidOperationException("DynamicsUrl is not configured in appsettings.json");

                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri($"{dynamicsUrl}/api/data/v9.2/");
                httpClient.Timeout = TimeSpan.FromMinutes(5);
                
                _logger.LogInformation("Shared HttpClient initialized with Dynamics URL: {BaseAddress}", httpClient.BaseAddress);
                
                return httpClient;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing shared HttpClient");
                throw;
            }
        });

        protected static HttpClient _httpClient => _lazyHttpClient.Value;

        protected DynamicsClientBase()
        {
        }

        protected async Task InitializeAuthenticationAsync()
        {
            var token = await TokenProvider.GetDynamicsUserToken();
            
            lock (_httpClient)
            {
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

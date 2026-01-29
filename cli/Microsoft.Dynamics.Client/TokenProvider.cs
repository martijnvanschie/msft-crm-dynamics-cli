using Microsoft.Dynamics.Core.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;

namespace Microsoft.Dynamics.Client
{
    public class TokenProvider
    {
        private static ILogger<TokenProvider> _logger = LoggerManager.GetLogger<TokenProvider>();

        public static string tenantId { get; set; } = "CliContext.TenantId";
        public static string clientId { get; set; } = "CliContext.ClientId";
        public static string clientSecret { get; set; } = "CliContext.ClientSecret";

        internal static async Task<AuthenticationResult> GetPartnerCenterClientToken()
        {
            var app = ConfidentialClientApplicationBuilder.Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority($"https://login.microsoftonline.com/{tenantId}")
                .Build();

            string[] scopes = { "https://api.partnercenter.microsoft.com/.default" };
            var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
            return result;
        }

        internal static async Task<AuthenticationResult> GetPartnerCenterUserToken()
        {
            string dynamicsUrl = "https://macaw.crm4.dynamics.com";
            string[] dynamicsScopes = { $"{dynamicsUrl}/user_impersonation" };

            var app = PublicClientApplicationBuilder.Create(clientId)
                .WithAuthority($"https://login.microsoftonline.com/{tenantId}")
                .WithRedirectUri("http://localhost")
                .Build();

            // Configure token cache to persist across sessions
            var cacheHelper = await CreateCacheHelperAsync();
            cacheHelper.RegisterCache(app.UserTokenCache);

            AuthenticationResult dynamicsResult;
            try
            {
                // Try to get token silently from cache
                var accounts = await app.GetAccountsAsync();
                dynamicsResult = await app.AcquireTokenSilent(dynamicsScopes, accounts.FirstOrDefault())
                    .ExecuteAsync();
                _logger.LogInformation("Token acquired silently from cache");
            }
            catch (MsalUiRequiredException)
            {
                // Cache miss or token expired, need interactive authentication
                dynamicsResult = await app.AcquireTokenInteractive(dynamicsScopes)
                    .ExecuteAsync();
                _logger.LogInformation("Token acquired interactively");
            }


            //var dynamicsResult = await app2.AcquireTokenInteractive(dynamicsScopes).ExecuteAsync();
            //Console.WriteLine("Dynamics Token acquired");
            return dynamicsResult;
        }

        /// <summary>
        /// Summary: The 403 error with client credentials occurs because there's no application user configured in Dynamics 365.
        /// Use GetUserToken() for now (which works), or set up the application user to use GetClientToken().
        /// </summary>
        /// <returns></returns>
        internal static async Task<AuthenticationResult> GetDynamicsClientToken()
        {
            var app = ConfidentialClientApplicationBuilder.Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority($"https://login.microsoftonline.com/{tenantId}")
                .Build();

            string dynamicsUrl = "https://macaw.crm4.dynamics.com";
            string[] dynamicsScopes = { $"{dynamicsUrl}/.default" };
            var dynamicsResult = await app.AcquireTokenForClient(dynamicsScopes).ExecuteAsync();

            _logger.LogInformation("Dynamics Token acquired");
            return dynamicsResult;
        }

        internal static async Task<AuthenticationResult> GetDynamicsUserToken()
        {
            _logger.LogInformation("Acquiring Dynamics user token...");
            string dynamicsUrl = "https://macaw.crm4.dynamics.com";
            string[] dynamicsScopes = { $"{dynamicsUrl}/user_impersonation" };

            var app = PublicClientApplicationBuilder.Create(clientId)
                .WithAuthority($"https://login.microsoftonline.com/{tenantId}")
                .WithRedirectUri("http://localhost")
                .Build();

            // Configure token cache to persist across sessions
            var cacheHelper = await CreateCacheHelperAsync();
            cacheHelper.RegisterCache(app.UserTokenCache);

            AuthenticationResult dynamicsResult;
            try
            {
                // Try to get token silently from cache
                var accounts = await app.GetAccountsAsync();
                dynamicsResult = await app.AcquireTokenSilent(dynamicsScopes, accounts.FirstOrDefault())
                    .ExecuteAsync();
                _logger.LogInformation("Token acquired silently from cache");
            }
            catch (MsalUiRequiredException)
            {
                // Cache miss or token expired, need interactive authentication
                dynamicsResult = await app.AcquireTokenInteractive(dynamicsScopes)
                    .ExecuteAsync();
                _logger.LogInformation("Token acquired interactively");
            }


            //var dynamicsResult = await app2.AcquireTokenInteractive(dynamicsScopes).ExecuteAsync();
            //Console.WriteLine("Dynamics Token acquired");
            return dynamicsResult;
        }

        private static async Task<MsalCacheHelper> CreateCacheHelperAsync()
        {
            var storageProperties = new StorageCreationPropertiesBuilder(
                "partner_center_cli.cache",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PartnerCenterCli"))
                .Build();

            var cacheHelper = await MsalCacheHelper.CreateAsync(storageProperties);
            return cacheHelper;
        }

        internal static async Task InvalidateCache()
        {
            _logger.LogInformation("Invalidating token cache...");

            var app = PublicClientApplicationBuilder.Create(clientId)
                .WithAuthority($"https://login.microsoftonline.com/{tenantId}")
                .WithRedirectUri("http://localhost")
                .Build();

            // Configure token cache
            var cacheHelper = await CreateCacheHelperAsync();
            cacheHelper.RegisterCache(app.UserTokenCache);

            // Remove all accounts from cache
            var accounts = await app.GetAccountsAsync();
            foreach (var account in accounts)
            {
                await app.RemoveAsync(account);
                _logger.LogInformation($"Removed account: {account.Username}");
            }

            // Optionally delete the cache file completely
            var cacheFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "PartnerCenterCli",
                "partner_center_cli.cache");

            if (File.Exists(cacheFilePath))
            {
                _logger.LogInformation("Deleting cache file at: {CacheFilePath}", cacheFilePath);
                File.Delete(cacheFilePath);
                _logger.LogInformation("Cache file deleted");
            }

            _logger.LogInformation("Token cache invalidated successfully");
        }
    }
}

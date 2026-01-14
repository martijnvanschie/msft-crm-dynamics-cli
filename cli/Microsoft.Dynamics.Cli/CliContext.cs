namespace Partner.Center.Cli
{
    internal class CliContext
    {
        private const string TenantIdEnvVar = "PARTNER_CENTER_TENANT_ID";
        private const string ClientIdEnvVar = "PARTNER_CENTER_CLIENT_ID";
        private const string ClientSecretEnvVar = "PARTNER_CENTER_CLIENT_SECRET";

        private static string? _tenantId;
        private static string? _clientId;
        private static string? _clientSecret;

        internal static string TenantId
        {
            get
            {
                if (_tenantId == null)
                {
                    _tenantId = Environment.GetEnvironmentVariable(TenantIdEnvVar)
                        ?? throw new InvalidOperationException($"{TenantIdEnvVar} environment variable not set");
                }
                return _tenantId;
            }
            set { _tenantId = value; }
        }

        internal static string ClientId
        {
            get
            {
                if (_clientId == null)
                {
                    _clientId = Environment.GetEnvironmentVariable(ClientIdEnvVar)
                        ?? throw new InvalidOperationException($"{ClientIdEnvVar} environment variable not set");
                }
                return _clientId;
            }
            set { _clientId = value; }  
        }

        internal static string ClientSecret
        {
            get
            {
                if (_clientSecret == null)
                {
                    _clientSecret = Environment.GetEnvironmentVariable(ClientSecretEnvVar)
                        ?? throw new InvalidOperationException($"{ClientSecretEnvVar} environment variable not set");
                }
                return _clientSecret;
            }
            set { _clientSecret = value; }  
        }
    }
}

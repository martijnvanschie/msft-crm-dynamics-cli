using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Dynamics.Core.Configuration
{
    public class ConfigManager
    {
        internal static IConfiguration _configuration = null!;
        private static ConfigurationRoot _rootConfig = new ConfigurationRoot();

        public static void Initiate(IConfiguration configuration)
        {
            _configuration = configuration;

            configuration.GetSection(nameof(Settings)).Bind(_rootConfig.Settings);
        }

        public static ConfigurationRoot GetConfiguration()
        {
            return _rootConfig;
        }
    }

    public class ConfigurationRoot
    {
        public Settings Settings { get; set; } = new Settings();
    }

    public class Settings
    {
        public string? DynamicsUrl { get; set; }
    }
}

using CodeTest.NET_Application.Common.Contracts.Services;
using Microsoft.Extensions.Configuration;

namespace CodeTest.NET_Application.Common.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private IConfiguration _configuration;

        public ConfigurationService()
        {
            _configuration = LoadConfiguration();
        }

        public string UserStoragePath
        {
            get { return _configuration["UserStoragePath"]; }
        }

        public static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                //.SetBasePath()
                .AddJsonFile("appsettings.json", optional: true,
                    reloadOnChange: true);
            return builder.Build();
        }
    }
}
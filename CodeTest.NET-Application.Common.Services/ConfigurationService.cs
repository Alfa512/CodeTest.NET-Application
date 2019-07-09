using System;
using System.IO;
using CodeTest.NET_Application.Common.Contracts.Services;
using Microsoft.Extensions.Configuration;

namespace CodeTest.NET_Application.Common.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private IConfiguration _configuration;

        public ConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string UserStoragePath
        {
            get { return _configuration["UserStoragePath"]; }
        }
    }
}
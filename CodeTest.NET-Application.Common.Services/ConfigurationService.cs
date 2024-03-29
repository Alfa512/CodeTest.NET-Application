﻿using System.IO;
using CodeTest.NET_Application.Common.Contracts.Services;
using Microsoft.Extensions.Configuration;

namespace CodeTest.NET_Application.Common.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfiguration _configuration;

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
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true);
            return builder.Build();
        }
    }
}
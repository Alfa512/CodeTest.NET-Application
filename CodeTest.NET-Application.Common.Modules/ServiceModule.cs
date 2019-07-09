using System;
using CodeTest.NET_Application.Common.Contracts.Services;
using CodeTest.NET_Application.Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeTest.NET_Application.Common.Modules
{
    public static class ServiceModule
    {
        public static void Register(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConfigurationService, ConfigurationService>();

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ITaskService, TaskService>();
        }
    }
}

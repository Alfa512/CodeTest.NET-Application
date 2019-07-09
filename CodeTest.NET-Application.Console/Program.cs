using System;
using CodeTest.NET_Application.Common.Contracts.Data;
using CodeTest.NET_Application.Common.Contracts.Repositories;
using Microsoft.Extensions.DependencyInjection;
using CodeTest.NET_Application.Common.Contracts.Services;
using CodeTest.NET_Application.Common.Services;
using CodeTest.NET_Application.Data;
using CodeTest.NET_Application.Data.Repositories;

namespace CodeTest.NET_Application.Console
{
    class Program
    {
        private static ServiceProvider collection;
        static void Main(string[] args)
        {
            InitializeDI();

            var userService = collection.GetService<IUserService>();
            var users = userService.GetAll();
        }

        static void InitializeDI()
        {
            //setup our DI
            collection = new ServiceCollection()
                .AddSingleton<IConfigurationService, ConfigurationService>()
                .AddSingleton<IUserService, UserService>()
                .AddSingleton<IUserRepository, UserRepository>()
                .AddSingleton<IDataContext, CsvContext>()
                .BuildServiceProvider();


        }
    }
}

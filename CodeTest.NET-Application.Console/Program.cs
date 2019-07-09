using System;
using System.IO;
using CodeTest.NET_Application.Common.Contracts.Data;
using CodeTest.NET_Application.Common.Contracts.Repositories;
using Microsoft.Extensions.DependencyInjection;
using CodeTest.NET_Application.Common.Contracts.Services;
using CodeTest.NET_Application.Common.Services;
using CodeTest.NET_Application.Data;
using CodeTest.NET_Application.Data.Repositories;
using Microsoft.Extensions.Configuration;

namespace CodeTest.NET_Application.Console
{
    public class Program
    {
        private static ServiceProvider collection;

        public static void Main(string[] args)
        {
            InitializeDI();

            var userService = collection.GetService<IUserService>();
            var users = userService.GetAll();

            foreach (var user in users)
            {
                System.Console.WriteLine($"{user.Id}, {user.FirstName}, {user.LastName}, {user.Age}");
            }

            System.Console.ReadKey();
        }

        private static void InitializeDI()
        {

            //setup our DI
            collection = new ServiceCollection()
                 //.AddSingleton<IConfiguration, ConfigurationBuilder>()
                .AddSingleton<IConfigurationService, ConfigurationService>()
                .AddSingleton<IUserService, UserService>()
                .AddSingleton<IUserRepository, UserRepository>()
                .AddSingleton<IDataContext, CsvContext>()
                .BuildServiceProvider();


        }

        public static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true,
                    reloadOnChange: true);
            return builder.Build();
        }
    }
}

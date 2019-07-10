﻿using System;
using System.IO;
using System.Linq;
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
        private static IUserService _userService;

        public static void Main(string[] args)
        {
            InitializeDI();

            _userService = collection.GetService<IUserService>();

            string command = "";
            var text = "Commands list:\r\n0 - Show All Users; 1 - Find By Id; 2 - Find By Last Name; 3 - Find Within Age Range";

            while (true)
            {
                try
                {

                
                WriteLine(text);
                command = System.Console.ReadLine();
                switch (command)
                {
                    case "0":
                        ShowAllUsers();
                        break;
                    case "1":
                        FindById();
                        break;
                    case "2":
                        FindByLastName();
                        break;
                    case "3":
                        FindWithinAgeRange();
                        break;
                }

                WriteLine();
                }

                catch (Exception e)
                {
                    WriteLine(e.Message);
                }
            }
        }

        private static void ShowAllUsers()
        {
            var users = _userService.GetAll();
            foreach (var user in users)
            {
                WriteLine($"{user.Id}, {user.FirstName}, {user.LastName}, {user.Age}");
            }
        }

        private static void FindById()
        {
            WriteLine("Enter Id: ");
            var id = Convert.ToInt32(System.Console.ReadLine());

            if (id <= 0)
                return;
            var user = _userService.GetById(id);
            if (user == null)
                return;
            WriteLine();
            WriteLine("----- ***** -----");
            WriteLine();
            WriteLine($"{user.Id}, {user.FirstName}, {user.LastName}, {user.Age}");
            WriteLine();
            WriteLine("----- ***** -----");
            WriteLine();
        }

        private static void FindByLastName()
        {
            WriteLine("Enter Last Name: ");
            var name = System.Console.ReadLine();

            if (string.IsNullOrEmpty(name))
                return;
            var users = _userService.FindByLastName(name);
            if (users == null || !users.Any())
                return;
            WriteLine();
            WriteLine("----- ***** -----");
            WriteLine();
            foreach (var user in users.ToList())
            {
                WriteLine($"{user.Id}, {user.FirstName}, {user.LastName}, {user.Age}");
            }
            WriteLine();
            WriteLine("----- ***** -----");
            WriteLine();
        }

        private static void FindWithinAgeRange()
        {
            WriteLine("Enter Age range (Ex. 18,35): ");
            var str = System.Console.ReadLine();

            var range = str.Split(new[] {',', ';', '/', '-'}, StringSplitOptions.RemoveEmptyEntries);

            if (range.Length != 2)
            {
                WriteLine("Please, enter correct range.");
                return;
            }

            var minAge = Convert.ToByte(range[0]);
            var maxAge = Convert.ToByte(range[1]);

            var users = _userService.FindWithinAgeRange(minAge, maxAge);
            if (users == null || !users.Any())
                return;
            WriteLine();
            WriteLine("----- ***** -----");
            WriteLine();
            foreach (var user in users.ToList())
            {
                WriteLine($"{user.Id}, {user.FirstName}, {user.LastName}, {user.Age}");
            }
            WriteLine();
            WriteLine("----- ***** -----");
            WriteLine();
        }

        private static void WriteLine()
        {
            System.Console.WriteLine();
        }
        private static void WriteLine(string line)
        {
            System.Console.WriteLine(line);
        }

        private static void InitializeDI()
        {
            collection = new ServiceCollection()
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
                .AddJsonFile("appsettings.json", true, true);
            return builder.Build();
        }
    }
}

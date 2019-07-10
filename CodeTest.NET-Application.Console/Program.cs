using System;
using System.IO;
using System.Linq;
using CodeTest.NET_Application.Business.Services;
using CodeTest.NET_Application.Common.Contracts.Data;
using CodeTest.NET_Application.Common.Contracts.Repositories;
using Microsoft.Extensions.DependencyInjection;
using CodeTest.NET_Application.Common.Contracts.Services;
using CodeTest.NET_Application.Common.Services;
using CodeTest.NET_Application.Data;
using CodeTest.NET_Application.Data.Repositories;

namespace CodeTest.NET_Application.Console
{
    public class Program
    {
        private static ServiceProvider _collection;
        private static IUserService _userService;

        public static void Main(string[] args)
        {
            InitializeDi();

            _userService = _collection.GetService<IUserService>();

            var text = "Commands list:\r\n0 - Show All Users; 1 - Find By Id; 2 - Find By Last Name; 3 - Find Within Age Range;\r\n4 - Load From File; 5 - Save To File; 6 - Exit";

            while (true)
            {
                try
                {
                    WriteLine(text);
                    var command = System.Console.ReadLine();
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
                        case "4":
                            LoadFromFile();
                            break;
                        case "5":
                            SaveToFile();
                            break;
                        case "6":
                            return;
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

            WriteIndent();

            WriteLine($"{user.Id}, {user.FirstName}, {user.LastName}, {user.Age}");

            WriteIndent();
        }

        private static void FindByLastName()
        {
            WriteLine("Enter Last Name: ");
            var name = System.Console.ReadLine();

            if (string.IsNullOrEmpty(name))
                return;
            var users = _userService.FindByLastName(name).ToList();
            if (!users.Any())
                return;

            WriteIndent();

            foreach (var user in users.ToList())
            {
                WriteLine($"{user.Id}, {user.FirstName}, {user.LastName}, {user.Age}");
            }

            WriteIndent();
        }

        private static void FindWithinAgeRange()
        {
            WriteLine("Enter Age range (Ex. 18,35): ");
            var str = System.Console.ReadLine();

            if(string.IsNullOrEmpty(str))
                return;

            var range = str.Split(new[] { ',', ';', '/', '-' }, StringSplitOptions.RemoveEmptyEntries);

            if (range.Length != 2)
            {
                WriteLine("Please, enter correct range.");
                return;
            }

            var minAge = Convert.ToByte(range[0]);
            var maxAge = Convert.ToByte(range[1]);

            var users = _userService.FindWithinAgeRange(minAge, maxAge).ToList();
            if (!users.Any())
                return;

            WriteIndent();

            foreach (var user in users.ToList())
            {
                WriteLine($"{user.Id}, {user.FirstName}, {user.LastName}, {user.Age}");
            }

            WriteIndent();
        }

        private static void LoadFromFile()
        {
            WriteLine("Enter File path: ");
            var path = System.Console.ReadLine();

            if (!File.Exists(path))
            {
                WriteLine("Please, enter valid file path.");
                return;
            }

            var users = _userService.LoadFromFile(path);
            if (users == null || !users.Any())
                return;

            WriteIndent();

            foreach (var user in users.ToList())
            {
                WriteLine($"{user.Id}, {user.FirstName}, {user.LastName}, {user.Age}");
            }

            WriteIndent();
        }

        private static void SaveToFile()
        {
            WriteLine("Enter File path: ");
            var path = System.Console.ReadLine();

            if (!Path.IsPathFullyQualified(path))
            {
                WriteLine("Please, enter valid file path.");
                return;
            }

            _userService.SaveToFile(path);

            WriteIndent();

        }

        private static void WriteLine()
        {
            System.Console.WriteLine();
        }

        private static void WriteLine(string line)
        {
            System.Console.WriteLine(line);
        }

        private static void WriteIndent()
        {
            WriteLine();
            WriteLine("----- ***** -----");
            WriteLine();
        }

        private static void InitializeDi()
        {
            _collection = new ServiceCollection()
                .AddSingleton<IConfigurationService, ConfigurationService>()
                .AddSingleton<IUserService, UserService>()
                .AddSingleton<IUserRepository, UserRepository>()
                .AddSingleton<IDataContext, CsvContext>()
                .BuildServiceProvider();
        }
    }
}

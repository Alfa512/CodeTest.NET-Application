using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeTest.NET_Application.Common.Contracts.Repositories;
using CodeTest.NET_Application.Common.Contracts.Services;
using CodeTest.NET_Application.Common.Services;
using CodeTest.NET_Application.Common.Models.ViewModel;
using CodeTest.NET_Application.Data.Models;
using CodeTest.NET_Application.Maps;

namespace CodeTest.NET_Application.Business.Services
{
    public class UserService : IUserService
    {
        private IUserRepository _repository;
        private CsvService _csvService;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
            _csvService = new CsvService();
        }

        public IEnumerable<UserVm> GetAll()
        {
            return DomainToViewList(_repository.All().ToList());
        }

        public UserVm GetById(int id)
        {
            return DomainToView(_repository.All().FirstOrDefault(u => u.ID == id));
        }

        public IEnumerable<UserVm> FindByLastName(string lastName)
        {
            return DomainToViewList(_repository.All()
                .Where(u => u.LastName.Contains(lastName, StringComparison.CurrentCultureIgnoreCase)).ToList());
        }

        public IEnumerable<UserVm> FindWithinAgeRange(int minAge, int maxAge)
        {
            return DomainToViewList(_repository.All().Where(u => u.Age >= minAge && u.Age <= maxAge).ToList());
        }


        public UserVm Create(UserVm userModel)
        {
            var user = UserMapper.Mapper().Map<User>(userModel);
            _repository.Add(user);
            if (user.ID > 0)
            {
                return DomainToView(user);
            }

            return null;
        }

        public UserVm Update(UserVm userParam)
        {
            var userModel = UserMapper.Mapper().Map<User>(userParam);

            var user = _repository.All().FirstOrDefault(u => u.ID == userModel.ID);

            if (user == null)
                throw new ApplicationException("User not found");

            user = ViewToDomain(userParam);

            _repository.Update(user);

            return GetById(userParam.Id);
        }

        public List<UserVm> LoadFromFile(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            var file = File.OpenRead(path);

            var users = _csvService.ReadFromStream<User>(file).ToList();
            _repository.AddRange(users);

            return DomainToViewList(users).ToList();
        }

        public void SaveToFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            var file = File.CreateText(path);
            file.WriteLine("ID,FirstName,LastName,Age");

            foreach (var user in _repository.All())
            {
                file.WriteLine($"{user.ID},{user.FirstName},{user.LastName},{user.Age}");
            }

            file.Close();
        }

        public void Delete(UserVm user)
        {
            ;
            if (user != null)
            {
                _repository.Delete(ViewToDomain(user));
            }
        }

        private IEnumerable<UserVm> DomainToViewList(List<User> users)
        {
            var vUsers = new List<UserVm>();

            if (users != null && users.Count > 0)
            {
                foreach (var user in users)
                {
                    vUsers.Add(UserMapper.Mapper().Map<UserVm>(user));
                }
            }

            return vUsers;
        }

        private List<User> ViewToDomainList(List<UserVm> users)
        {
            var vUsers = new List<User>();

            if (users != null && users.Count > 0)
            {
                foreach (var user in users)
                {
                    vUsers.Add(UserMapper.Mapper().Map<User>(user));
                }
            }

            return vUsers;
        }

        private UserVm DomainToView(User user)
        {
            return UserMapper.Mapper().Map<UserVm>(user);
        }

        private User ViewToDomain(UserVm user)
        {
            return UserMapper.Mapper().Map<User>(user);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using CodeTest.NET_Application.Business.Services;
using CodeTest.NET_Application.Common.Contracts.Repositories;
using CodeTest.NET_Application.Common.Contracts.Services;
using CodeTest.NET_Application.Common.Models.Enums;
using CodeTest.NET_Application.Common.Models.ViewModel;
using CodeTest.NET_Application.Data.Models;
using CodeTest.NET_Application.Maps;

namespace CodeTest.NET_Application.Common.Services
{
    public class UserService : IUserService
    {
        private IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<UserVm> OrderUsers(List<UserVm> users, OrderByUserFilter filter)
        {
            switch (filter)
            {
                case OrderByUserFilter.ByNameAsk:
                    return users.OrderBy(u => u.FirstName);
                case OrderByUserFilter.ByNameDesc:
                    return users.OrderByDescending(u => u.FirstName);
                case OrderByUserFilter.ByLastNameAsc:
                    return users.OrderBy(u => u.LastName);
                case OrderByUserFilter.ByLastNameDesc:
                    return users.OrderByDescending(u => u.LastName);


                default:
                {
                    return users.OrderBy(u => u.FirstName).ThenBy(u => u.LastName);
                }
            }
        }

        public IEnumerable<UserVm> GetAll()
        {
            return DomainToViewList(_repository.All().ToList());
        }

        public UserVm GetById(int id)
        {
            return DomainToView(_repository.All().FirstOrDefault(u => u.ID == id));
        }

        public UserVm GetFirstByName(string firstName)
        {
            return DomainToView(_repository.All().FirstOrDefault(u => string.Equals(u.FirstName, firstName, StringComparison.CurrentCultureIgnoreCase)));
        }

        public IEnumerable<UserVm> FindByLastName(string lastName)
        {
            return DomainToViewList(_repository.All().Where(u => u.LastName.Contains(lastName, StringComparison.CurrentCultureIgnoreCase)).ToList());
        }


        public UserVm Create(UserVm userModel)
        {
            var user = UserMapper.Mapper().Map<User>(userModel);
            _repository.Add(user);
            //_repository.SaveCganges();
            if (user.ID > 0)
            {
                return DomainToView(user);
            }

            return null;
        }

        /*ToDo Not Implemented*/
        public UserVm Update(UserVm userParam)
        {
            var userModel = UserMapper.Mapper().Map<User>(userParam);

            var user = _repository.All().FirstOrDefault(u => u.ID == userModel.ID);

            if (user == null)
                throw new ApplicationException("User not found");

            user = ViewToDomain(userParam);

            _repository.Update(user);
            //_repository.SaveCganges();

            return GetById(userParam.Id);
        }
        
        /*ToDo Not Implemented*/
        public List<UserVm> LoadFromFile(byte[] content)
        {
            var csvServices = new CsvService();
            var users = csvServices.ParseUsers(content).ToList();

            foreach (var user in users)
            {
                _repository.Add(ViewToDomain(user));
            }

            return users;
        }

        /*ToDo Not Implemented*/
        public List<UserVm> LoadFromText(string text)
        {
            var csvServices = new CsvService();
            var users = csvServices.ParseUsers(text).ToList();

            foreach (var user in users)
            {
                _repository.Add(ViewToDomain(user));
            }

            return users;
        }

        /*ToDo Wasn't Tested*/
        public void Delete(int id)
        {
            var user = _repository.All().FirstOrDefault(u => u.ID == id);
            if (user != null)
            {
                _repository.Delete(user);
                //_repository.SaveCganges();
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
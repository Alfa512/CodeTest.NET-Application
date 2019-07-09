using System.Collections.Generic;
using CodeTest.NET_Application.Common.Models.Enums;
using CodeTest.NET_Application.Common.Models.ViewModel;

namespace CodeTest.NET_Application.Common.Contracts.Services
{
    public interface IUserService
    {
        IEnumerable<UserVm> GetAll();
        UserVm GetById(int id);
        UserVm GetFirstByName(string lastName);
        IEnumerable<UserVm> FindByLastName(string lastName);
        UserVm Create(UserVm user);
        UserVm Update(UserVm user);
        List<UserVm> LoadFromFile(byte[] content);
        List<UserVm> LoadFromText(string text);
        void Delete(int id);
        IEnumerable<UserVm> OrderUsers(List<UserVm> users, OrderByUserFilter filter);
    }
}
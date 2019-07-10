using System.Collections.Generic;
using CodeTest.NET_Application.Common.Models.ViewModel;

namespace CodeTest.NET_Application.Common.Contracts.Services
{
    public interface IUserService
    {
        IEnumerable<UserVm> GetAll();
        UserVm GetById(int id);
        IEnumerable<UserVm> FindByLastName(string lastName);
        IEnumerable<UserVm> FindWithinAgeRange(int minAge, int maxAge);
        UserVm Create(UserVm user);
        UserVm Update(UserVm user);
        List<UserVm> LoadFromFile(string path);
        void SaveToFile(string path);

        void Delete(UserVm user);
    }
}
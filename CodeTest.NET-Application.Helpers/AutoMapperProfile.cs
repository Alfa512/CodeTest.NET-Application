using AutoMapper;
using CodeTest.NET_Application.Common.Models.ViewModel;
using CodeTest.NET_Application.Data.Models;

namespace CodeTest.NET_Application.Common.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserVm>();
            CreateMap<UserVm, User>();
        }
    }
}

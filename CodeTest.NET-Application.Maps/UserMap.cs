using AutoMapper;
using CodeTest.NET_Application.Common.Models.ViewModel;
using CodeTest.NET_Application.Data.Models;

namespace CodeTest.NET_Application.Maps
{
    public static class UserMapper
    {
        public static IMapper Mapper()
        {
            var mapperConfig = new MapperConfiguration(
                configuration => { configuration.CreateMap<User, UserVm>(); });

            mapperConfig.AssertConfigurationIsValid();

            return mapperConfig.CreateMapper();
        }

    }
}

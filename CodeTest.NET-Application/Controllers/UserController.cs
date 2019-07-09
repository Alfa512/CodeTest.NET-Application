using System.Collections.Generic;
using System.Linq;
using CodeTest.NET_Application.Common.Contracts.Services;
using CodeTest.NET_Application.Common.Models.Enums;
using CodeTest.NET_Application.Common.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeTest.NET_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [Route("list")]
        public List<UserVm> List(int? orderBy)
        {
            var sort = orderBy != null ? (OrderByUserFilter)orderBy : OrderByUserFilter.ByNameAsk;
            var users = _userService.OrderUsers(_userService.GetAll().ToList(), sort);
            return users.ToList();
        }

        [Route("getbyid")]
        public UserVm GetById(int id)
        {
            var user = _userService.GetById(id);
            return user;
        }

        [Route("getbyid")]
        public List<UserVm> FindByLastName(string lastName)
        {
            var users = _userService.FindByLastName(lastName).ToList();
            return users;
        }

        [AllowAnonymous]
        [Route("create")]
        [HttpPost]
        public UserVm Create([FromBody]UserVm user)
        {
            var newUser = _userService.Create(user);

            return newUser;
        }

        [AllowAnonymous]
        [Route("update")]
        public UserVm Update(UserVm user)
        {
            var mUser = _userService.Update(user);
            return mUser;
        }

        [AllowAnonymous]
        [Route("loadfromfile")]
        public List<UserVm> LoadFromFile(byte[] content)
        {
            var users = _userService.LoadFromFile(content);
            return users;
        }

        [AllowAnonymous]
        [Route("loadfromtext")]
        public List<UserVm> LoadFromText(string text)
        {
            var users = _userService.LoadFromText(text);
            return users;
        }

        [AllowAnonymous]
        [Route("delete")]
        public void Delete(int id)
        {
            _userService.Delete(id);
        }
    }
}

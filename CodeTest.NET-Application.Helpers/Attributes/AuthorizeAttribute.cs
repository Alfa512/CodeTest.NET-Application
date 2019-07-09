//using System;
//using System.Collections.Generic;
//using System.Security.Claims;
//using System.Text;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;

//namespace CodeTest.NET_Application.Common.Helpers.Attributes
//{
//    class AuthorizeAttribute : TypeFilterAttribute
//    {
//        public AuthorizeAttribute(string claimType, string claimValue) : base(typeof(AuthorizeFilter))
//        {
//            Arguments = new object[] { new Claim(claimType, claimValue) };
//        }
//    }

//    public class AuthorizeFilter : IAuthorizationFilter
//    {
//        readonly Claim _claim;

//        public AuthorizeFilter(Claim claim)
//        {
//            _claim = claim;
//        }

//        public void OnAuthorization(AuthorizationFilterContext context)
//        {
//            var hasClaim = context.HttpContext.User.Claims.Any(c => c.Type == _claim.Type && c.Value == _claim.Value);
//            if (!hasClaim)
//            {
//                context.Result = new ForbidResult();
//            }
//        }
//    }
//}

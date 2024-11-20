using LuckyProject.Lib.Basics.Models;
using LuckyProject.Lib.Web.Constants;
using LuckyProject.Lib.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;

namespace LuckyProject.Lib.Web.Attributes
{
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Method,
        AllowMultiple = true,
        Inherited = true)]
    public class LpJwtAuthorizeAttribute : AuthorizeAttribute
    {
        public List<LpAuthRequirement> Requirements { get; }

        public LpJwtAuthorizeAttribute(string s)
            : base(LpWebConstants.Auth.Jwt.AuthoriaztionPolicyPrefix + s)
        {
            AuthenticationSchemes = LpWebConstants.Auth.Jwt.AuthenticationScheme;
            Requirements = LpJwtHelper.ParseAuthRequirements(s);
        }
    }
}

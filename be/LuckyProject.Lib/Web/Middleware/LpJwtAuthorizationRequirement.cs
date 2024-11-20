using LuckyProject.Lib.Basics.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace LuckyProject.Lib.Web.Middleware
{
    public class LpJwtAuthorizationRequirement : IAuthorizationRequirement
    {
        public List<LpAuthRequirement> Requirements { get; init; }
    }
}

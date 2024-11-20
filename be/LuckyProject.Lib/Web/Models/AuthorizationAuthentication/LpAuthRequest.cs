using LuckyProject.Lib.Basics.Models;
using System.Collections.Generic;

namespace LuckyProject.Lib.Web.Models.Requests
{
    public class LpAuthRequest
    {
        public string Origin { get; init; }
        public string Token { get; init; }
        public List<LpAuthRequirement> Requirements { get; init; }
    }
}

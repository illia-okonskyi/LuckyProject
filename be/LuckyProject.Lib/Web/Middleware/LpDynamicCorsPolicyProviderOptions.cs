using System.Collections.Generic;

namespace LuckyProject.Lib.Web.Middleware
{
    public class LpDynamicCorsPolicyProviderOptions
    {
        public HashSet<string> SelfOrigins { get; set; }
    }
}

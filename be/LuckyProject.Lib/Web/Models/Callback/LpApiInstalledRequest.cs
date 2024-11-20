using System;

namespace LuckyProject.Lib.Web.Models.Callback
{
    public class LpApiInstalledRequest
    {
        public Guid ApiId { get; init; }
        public string ClientId { get; init; }
        public string ClientSecret { get; init; }
        public Guid UserId { get; init; }
    }
}

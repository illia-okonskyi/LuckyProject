using System;
using System.Collections.Generic;

namespace LuckyProject.AuthServer.Services.Users.Responses
{
    public class AuthorizeResponse
    {
        public UserResponse User { get; init; }
        public List<Guid> UserRoleIds {  get; init; }
    }
}

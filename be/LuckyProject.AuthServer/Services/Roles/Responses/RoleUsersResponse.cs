using LuckyProject.Lib.Basics.Collections;
using System;
using System.Collections.Generic;

namespace LuckyProject.AuthServer.Services.Roles.Responses
{
    public class RoleUsersResponse
    {
        public class RoleUser
        {
            public Guid Id { get; init; }
            public string UserName { get; init; }
            public string Email { get; init; }
            public string FullName { get; init; }
            public bool IsSealed { get; init; }
        }

        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public PaginatedList<RoleUser> Users { get; init; }
    }
}

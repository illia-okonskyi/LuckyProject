using LuckyProject.Lib.Basics.Helpers;
using LuckyProject.Lib.Basics.Models;
using LuckyProject.Lib.Web.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace LuckyProject.Lib.Swagger.Filters
{
    public class LpJwtAuthorizeOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var attributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<LpJwtAuthorizeAttribute>()
                .ToList();
            if (attributes.Count == 0)
            {
                return;
            }

            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });
            var requiredScopes = attributes
                .Select(attr => attr.Policy)
                .Distinct()
                .ToList();
            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new ()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new()
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "lp-auth"
                            }
                        },
                        requiredScopes.ToList()
                    }
                }
            };

            var rDescs = attributes
                .SelectMany(a => a.Requirements)
                .GroupBy(r => r.PermissionFullName)
                .Select(AuthRequirementToDescription)
                .ToList();
            var authDescription = $"\r\n\r\n**Auth:**\r\n{string.Join(string.Empty, rDescs)}\r\n";
            operation.Description = operation.Description + authDescription;
        }

        private string AuthRequirementToDescription(IGrouping<string, LpAuthRequirement> g)
        {
            var type = AuthorizationHelper.GetPermissionType(g.Key);
            var parameters = string.Empty;
            if (type == LpAuthPermissionType.Level)
            {
                parameters = ": \r\n" +
                    string.Join("\r\n", g.Select(r => $"\t- {r.ExpectedValue}"));
            }
            else if (type == LpAuthPermissionType.Passkey)
            {
                var pks = g.SelectMany(r => (HashSet<string>)r.ExpectedValue).ToHashSet();
                parameters = ": \r\n" + string.Join("\r\n", pks.Select(pk => $"\t- {pk}"));
            }

            return $"- {g.Key}{parameters}\r\n";
        }
    }
}

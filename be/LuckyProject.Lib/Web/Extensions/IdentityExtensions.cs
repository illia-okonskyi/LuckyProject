using FluentValidation;
using LuckyProject.Lib.Basics.Constants;
using LuckyProject.Lib.Web.Constants;
using LuckyProject.Lib.Web.Helpers;
using LuckyProject.Lib.Web.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Claims;

namespace LuckyProject.Lib.Web.Extensions
{
    public static class IdentityExtensions
    {
        public static void EnsureSucceded(this IdentityResult ir)
        {
            if (ir.Succeeded)
            {
                return;
            }

            var irMessage = string.Join("; ", ir.Errors.Select(e => $"{e.Code}/{e.Description}"));
            throw new InvalidOperationException($"Error - {irMessage}");
        }


        /// <summary>
        /// Returns true if has errors and context updated
        /// </summary>
        public static bool UpdateValidationContext<T>(
            this IdentityResult ir,
            string propertyName,
            string propertyPath,
            ValidationContext<T> ctx)
        {
            if (ir.Succeeded)
            {
                return false;
            }

            foreach (var e in ir.Errors)
            {
                ctx.AddFailure(new FluentValidation.Results.ValidationFailure()
                {
                    PropertyName = propertyName,
                    ErrorCode = ValidationErrorCodes.MessagesOnly,
                    ErrorMessage = e.Description,
                    FormattedMessagePlaceholderValues = new() { { "PropertyPath", propertyPath } }
                });
            }

            return true;
        }

        public static LpIdentity ToLpIdentity(this ClaimsIdentity ci)
        {
            if (ci == null)
            {
                return null;
            }

            if (!ci.IsAuthenticated)
            {
                return null;
            }

            var result = new LpIdentity();
            foreach (var claim in ci.Claims)
            {
                var value = claim.Value;
                switch (claim.Type)
                {
                    case LpWebConstants.Auth.Jwt.Claims.Subject:
                        if (Guid.TryParse(value, out var userId))
                        {
                            result.UserId = userId;
                        }
                        break;
                    case LpWebConstants.Auth.Jwt.Claims.ClientId:
                        result.ClientId = value;
                        result.ClientType = LpApiClientTypeHelper.GetClientType(value);
                        result.ClientName = LpApiClientTypeHelper.GetClientName(value);
                        break;
                    case LpWebConstants.Auth.Jwt.Claims.SessionId:
                        if (Guid.TryParse(value, out var sessionId))
                        {
                            result.SessionId = sessionId;
                        }
                        break;
                    case LpWebConstants.Auth.Jwt.Claims.UserName:
                        result.UserName = value;
                        break;
                    case LpWebConstants.Auth.Jwt.Claims.Email:
                        result.UserEmail = value;
                        break;
                    case LpWebConstants.Auth.Jwt.Claims.PhoneNumber:
                        result.UserPhoneNumber = value;
                        break;
                    case LpWebConstants.Auth.Jwt.Claims.FullName:
                        result.UserFullName = value;
                        break;
                    case LpWebConstants.Auth.Jwt.Claims.TelegramUserName:
                        result.UserTelegramUserName = value;
                        break;
                    case LpWebConstants.Auth.Jwt.Claims.PreferredLocale:
                        result.UserPreferredLocale = value;
                        break;
                    case LpWebConstants.Auth.Jwt.Claims.Role:
                        if (Guid.TryParse(value, out var roleId))
                        {
                            result.UserRoleIds.Add(roleId);
                        }
                        break;
                    default:
                        break;
                }
            }

            if (string.IsNullOrEmpty(result.ClientId) ||
                result.UserId == default ||
                result.SessionId == default)
            {
                return null;
            }

            return result;
        }

        public static LpIdentity GetLpIdentity(this ClaimsPrincipal cp)
        {
            return cp.Identities.Select(ToLpIdentity).FirstOrDefault(i => i != null);
        }
    }
}

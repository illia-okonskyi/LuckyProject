using LuckyProject.AuthServer.DbLayer;
using LuckyProject.AuthServer.Helpers;
using LuckyProject.AuthServer.Services.Roles.Requests;
using LuckyProject.AuthServer.Services.Roles.Responses;
using LuckyProject.AuthServer.Services.Roles.Validators;
using LuckyProject.AuthServer.Services.Sessions;
using LuckyProject.AuthServer.Services.Users.Responses;
using LuckyProject.Lib.Basics.Collections;
using LuckyProject.Lib.Basics.Constants;
using LuckyProject.Lib.Basics.Extensions;
using LuckyProject.Lib.Basics.Models;
using LuckyProject.Lib.Basics.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.AuthServer.Services.Roles
{
    public class RolesService : IRolesService
    {
        #region Internals & ctor
        private readonly ILookupNormalizer normalizer;
        private readonly RoleManager<AuthServerRole> roleManager;
        private readonly AuthServerDbContext dbContext;
        private readonly IValidationService validationService;
        private readonly ILpAuthorizationService authService;
        private readonly ILpLocalizationService localizationService;
        private readonly IAuthSessionManager authSessionManager;

        public RolesService(
            RoleManager<AuthServerRole> roleManager,
            AuthServerDbContext dbContext,
            IValidationService validationService,
            ILpAuthorizationService authService,
            ILpLocalizationService localizationService,
            IAuthSessionManager authSessionManager)
        {
            normalizer = roleManager.KeyNormalizer;
            this.roleManager = roleManager;
            this.dbContext = dbContext;
            this.validationService = validationService;
            this.authService = authService;
            this.localizationService = localizationService;
            this.authSessionManager = authSessionManager;
        }
        #endregion

        #region CRUD
        public async Task<RoleResponse> CreateRoleAsync(CreateUpdateRoleRequest request)
        {
            request.PrepareRequest(normalizer);
            var normilizedRoleNames = await GetAllNormilizedRoleNamesAsync();
            validationService.EnsureValid(
                request,
                new CreateUpdateRoleRequestValidator(normilizedRoleNames));

            var role = dbContext.Roles.Add(new()
            {
                Name = request.Name,
                NormalizedName = request.NormalizedName,
                Description = request.Description,
                IsSealed = request.IsSealed,
                ConcurrencyStamp = DateTime.UtcNow.ToString("o")
            }).Entity;
            await dbContext.SaveChangesAsync();
            return new RoleResponse
            {
                Id =  role.Id,
                Name = role.Name,
                NormalizedName = role.NormalizedName,
                Description = role.Description,
                IsSealed = role.IsSealed
            };
        }

        public async Task<PaginatedList<RoleResponse>> GetRolesAsync(
            LpFilterOrderPaginationRequest request,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var q = dbContext.Roles.AsNoTracking();
            var nameFilter = request.GetFilter<string>("name");
            if (!string.IsNullOrEmpty(nameFilter))
            {
                q = q.Where(r => r.Name.Contains(nameFilter));
            }
            var isSealedFilter = request.GetFilter<bool?>("sealed");
            if (isSealedFilter.HasValue)
            {
                q = q.Where(r => r.IsSealed == isSealedFilter.Value);
            }

            if (request.Order == "id-asc")
            {
                q = q.OrderBy(r => r.Id);
            }
            else if (request.Order == "id-desc")
            {
                q = q.OrderByDescending(r => r.Id);
            }
            else if (request.Order == "name-asc")
            {
                q = q.OrderBy(r => r.Name);
            }
            else if (request.Order == "name-desc")
            {
                q = q.OrderByDescending(r => r.Name);
            }
            else
            {
                q = q.OrderBy(r => r.Id);
            }

            return await q.Select(r => new RoleResponse
            {
                Id = r.Id,
                Name = r.Name,
                NormalizedName = r.NormalizedName,
                Description = r.Description,
                IsSealed = r.IsSealed
            })
            .ToPaginatedListAsync(pageSize, request.Page, false, cancellationToken);
        }

        public async Task<RoleResponse> GetRoleByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await dbContext.Roles
                .AsNoTracking()
                .Where(r => r.Id == id)
                .Select(r => new RoleResponse
                {
                    Id = r.Id,
                    Name = r.Name,
                    NormalizedName = r.NormalizedName,
                    Description = r.Description,
                    IsSealed = r.IsSealed
                })
                .FirstOrDefaultAsync(cancellationToken);
            if (result == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(id),
                    ValidationErrorCodes.NotFound);
            }

            return result;
        }

        public async Task<RoleResponse> GetRoleByNameAsync(
            string name,
            CancellationToken cancellationToken = default)
        {
            var normilizedName = normalizer.NormalizeName(name);
            var result = await dbContext.Roles
                .AsNoTracking()
                .Where(r => r.NormalizedName == normilizedName)
                .Select(r => new RoleResponse
                {
                    Id = r.Id,
                    Name = r.Name,
                    NormalizedName = r.NormalizedName,
                    Description = r.Description,
                    IsSealed = r.IsSealed
                })
                .FirstOrDefaultAsync(cancellationToken);
            if (result == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(name),
                    ValidationErrorCodes.NotFound);
            }

            return result;
        }

        public async Task<RoleResponse> UpdateRoleAsync(CreateUpdateRoleRequest request)
        {
            var role = await dbContext.Roles.FirstOrDefaultAsync(r => r.Id == request.Id);
            if (role == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(request.Id),
                    ValidationErrorCodes.NotFound);
            }

            // NOTE: Ensure we are not changing sealed role
            if (!request.IgnoreSealed && role.IsSealed)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(request.Id),
                    ValidationErrorCodes.AccessDenied);
            }

            var normalizedRoleNames = await GetAllNormilizedRoleNamesAsync();
            normalizedRoleNames = normalizedRoleNames.Except([role.NormalizedName]).ToHashSet();
            request.PrepareRequest(normalizer);
            validationService.EnsureValid(
                request,
                new CreateUpdateRoleRequestValidator(normalizedRoleNames));

            role.Name = request.Name;
            role.NormalizedName = request.NormalizedName;
            role.Description = request.Description;
            await dbContext.SaveChangesAsync();
            return new RoleResponse
            {
                Id = role.Id,
                Name = role.Name,
                NormalizedName = role.NormalizedName,
                Description = role.Description,
                IsSealed = role.IsSealed
            };
        }

        public async Task DeleteRoleAsync(Guid id, bool ignoreSealed = false)
        {
            var role = await roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(id),
                    ValidationErrorCodes.NotFound);
            }

            // NOTE: Ensure we are not deleting sealed role
            if (!ignoreSealed && role.IsSealed)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(id),
                    ValidationErrorCodes.AccessDenied);
            }

            await roleManager.DeleteAsync(role);
        }
        #endregion

        #region Role-Users management
        public async Task<PaginatedList<UserResponse>> GetRoleUsersAsync(
            Guid roleId,
            LpFilterOrderPaginationRequest request,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var role = await dbContext.Roles
                .AsNoTracking()
                .Where(r => r.Id == roleId)
                .Select(r => new
                {
                    r.Id,
                    r.IsSealed,
                })
                .FirstOrDefaultAsync(cancellationToken);
            if (role == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(roleId),
                    ValidationErrorCodes.NotFound);
            }

            var userIds = await dbContext.UserRoles
                .AsNoTracking()
                .Where(ur => ur.RoleId == roleId)
                .Select(ur => ur.UserId)
                .ToListAsync(cancellationToken);
            var q = dbContext.Users
                .AsNoTracking()
                .Where(u => userIds.Contains(u.Id));
            var userNameFilter = request.GetFilter<string>("userName");
            if (!string.IsNullOrEmpty(userNameFilter))
            {
                q = q.Where(u => u.UserName.Contains(userNameFilter));
            }
            var emailFilter = request.GetFilter<string>("email");
            if (!string.IsNullOrEmpty(emailFilter))
            {
                q = q.Where(u => u.Email.Contains(emailFilter));
            }
            var phoneNumberFilter = request.GetFilter<string>("phoneNumber");
            if (!string.IsNullOrEmpty(phoneNumberFilter))
            {
                q = q.Where(u => u.PhoneNumber.Contains(phoneNumberFilter));
            }
            var fullNameFilter = request.GetFilter<string>("fullName");
            if (!string.IsNullOrEmpty(fullNameFilter))
            {
                q = q.Where(u => u.FullName.Contains(fullNameFilter));
            }
            var tgUserNameFilter = request.GetFilter<string>("tgUserName");
            if (!string.IsNullOrEmpty(tgUserNameFilter))
            {
                q = q.Where(u => u.TelegramUserName.Contains(tgUserNameFilter));
            }
            var preferredLocaleFilter = request.GetFilter<string>("preferredLocale");
            if (!string.IsNullOrEmpty(preferredLocaleFilter))
            {
                q = q.Where(u => u.PreferredLocale == preferredLocaleFilter);
            }
            var isMachineFilter = request.GetFilter<bool?>("machine");
            if (isMachineFilter.HasValue)
            {
                q = q.Where(u => string.IsNullOrEmpty(u.MachineClientId) == isMachineFilter.Value);
            }
            var isSealedFilter = request.GetFilter<bool?>("sealed");
            if (isSealedFilter.HasValue)
            {
                q = q.Where(u => u.IsSealed == isSealedFilter.Value);
            }

            if (request.Order == "id-asc")
            {
                q = q.OrderBy(u => u.Id);
            }
            else if (request.Order == "id-desc")
            {
                q = q.OrderByDescending(u => u.Id);
            }
            else if (request.Order == "userName-asc")
            {
                q = q.OrderBy(u => u.UserName);
            }
            else if (request.Order == "userName-desc")
            {
                q = q.OrderByDescending(u => u.UserName);
            }
            else if (request.Order == "email-asc")
            {
                q = q.OrderBy(u => u.Email);
            }
            else if (request.Order == "email-desc")
            {
                q = q.OrderByDescending(u => u.Email);
            }
            else if (request.Order == "phoneNumber-asc")
            {
                q = q.OrderBy(u => u.PhoneNumber);
            }
            else if (request.Order == "phoneNumber-desc")
            {
                q = q.OrderByDescending(u => u.PhoneNumber);
            }
            else if (request.Order == "fullName-asc")
            {
                q = q.OrderBy(u => u.FullName);
            }
            else if (request.Order == "fullName-desc")
            {
                q = q.OrderByDescending(u => u.FullName);
            }
            else if (request.Order == "tgUserName-asc")
            {
                q = q.OrderBy(u => u.TelegramUserName);
            }
            else if (request.Order == "tgUserName-desc")
            {
                q = q.OrderByDescending(u => u.TelegramUserName);
            }
            else if (request.Order == "preferredLocale-asc")
            {
                q = q.OrderBy(u => u.PreferredLocale);
            }
            else if (request.Order == "preferredLocale-desc")
            {
                q = q.OrderByDescending(u => u.PreferredLocale);
            }
            else
            {
                q = q.OrderBy(r => r.Id);
            }

            var users = await q.Select(u => new UserResponse
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                NormalizedEmail = u.NormalizedEmail,
                PhoneNumber = u.PhoneNumber,
                FullName = u.FullName,
                TelegramUserName = u.TelegramUserName,
                PreferredLocale = u.PreferredLocale,
                MachineClientId = u.MachineClientId,
                LockoutEndUtc = u.LockoutEnd,
                IsSealed = u.IsSealed
            })
            .ToPaginatedListAsync(pageSize, request.Page, false, cancellationToken);
            users.Items.ForEach(u => u.PreferredLocaleDisplayName = localizationService
                .GetLocaleInfo(u.PreferredLocale).DisplayName);
            return users;
        }

        public async Task AssignUserToRoleAsync(RoleUserRequest request)
        {
            var role = await dbContext.Roles
                .AsNoTracking()
                .Where(r => r.Id == request.RoleId)
                .Select(r => new { r.Id, r.IsSealed })
                .FirstOrDefaultAsync();
            if (role == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(request.RoleId),
                    ValidationErrorCodes.NotFound);
            }

            // NOTE: Ensure we are not changing sealed role
            if (!request.IgnoreSealed && role.IsSealed)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(request.RoleId),
                    ValidationErrorCodes.AccessDenied);
            }

            var userExists = await dbContext.Users
                .AsNoTracking()
                .AnyAsync(u => u.Id == request.UserId);
            if (!userExists)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(request.UserId),
                    ValidationErrorCodes.NotFound);
            }

            dbContext.UserRoles.Add(new()
            {
                RoleId = request.RoleId,
                UserId = request.UserId
            });
            await dbContext.SaveChangesAsync();
            await authSessionManager.OnUserChangedAsync(request.UserId);
        }

        public async Task DeleteUserFromRoleAsync(RoleUserRequest request)
        {
            var role = await dbContext.Roles
                .AsNoTracking()
                .Where(r => r.Id == request.RoleId)
                .Select(r => new { r.Id, r.IsSealed })
                .FirstOrDefaultAsync();
            if (role == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(request.RoleId),
                    ValidationErrorCodes.NotFound);
            }

            // NOTE: Ensure we are not changing sealed role
            if (!request.IgnoreSealed && role.IsSealed)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(request.RoleId),
                    ValidationErrorCodes.AccessDenied);
            }

            dbContext.UserRoles.Remove(new()
            {
                RoleId = request.RoleId,
                UserId = request.UserId
            });
            await dbContext.SaveChangesAsync();
            await authSessionManager.OnUserChangedAsync(request.UserId);
        }
        #endregion

        #region Role-Permissions management
        public async Task<PaginatedList<RolePermissionsResponse>> GetRolePermissionsAsync(
            Guid roleId,
            LpFilterOrderPaginationRequest request,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var role = await dbContext.Roles
                .AsNoTracking()
                .Where(r => r.Id == roleId)
                .Select(r => new
                {
                    r.Id,
                    r.IsSealed,
                })
                .FirstOrDefaultAsync(cancellationToken);
            if (role == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(roleId),
                    ValidationErrorCodes.NotFound);
            }

            var permissionIds = await dbContext.RolePermissions
                .AsNoTracking()
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.PermissionId)
                .ToListAsync(cancellationToken);
            var q = dbContext.Permissions
                .AsNoTracking()
                .Where(p => permissionIds.Contains(p.Id));
            var typeFilter = request.GetEnumFilter<LpAuthPermissionType>("type");
            if (typeFilter.HasValue)
            {
                var prefix = authService.GetPermissionTypePrefix(typeFilter.Value);
                q = q.Where(p => p.FullName.StartsWith(prefix));
            }
            var nameFilter = request.GetFilter<string>("name");
            if (!string.IsNullOrEmpty(nameFilter))
            {
                q = q.Where(p => p.FullName.Substring(1).Contains(nameFilter));
            }

            if (request.Order == "id-asc")
            {
                q = q.OrderBy(p => p.Id);
            }
            else if (request.Order == "id-desc")
            {
                q = q.OrderByDescending(p => p.Id);
            }
            else if (request.Order == "type-asc")
            {
                q = q.OrderBy(p => p.FullName.Substring(0, 1));
            }
            else if (request.Order == "type-desc")
            {
                q = q.OrderByDescending(p => p.FullName.Substring(0, 1));
            }
            else if (request.Order == "name-asc")
            {
                q = q.OrderBy(p => p.FullName.Substring(1));
            }
            else if (request.Order == "name-desc")
            {
                q = q.OrderByDescending(p => p.FullName.Substring(1));
            }
            else
            {
                q = q.OrderBy(p => p.Id);
            }

            var permissions = await q
                .Select(p => new RolePermissionsResponse
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    Description = p.Description,
                    IsSealed = p.IsSealed,
                })
                .ToPaginatedListAsync(pageSize, request.Page, false, cancellationToken);
            permissions.Items.ForEach(p => p.Fullfill(authService));
            return permissions;
        }

        public async Task AssignPermissionToRoleAsync(RolePermissionRequest request)
        {
            var role = await dbContext.Roles
                .AsNoTracking()
                .Where(r => r.Id == request.RoleId)
                .Select(r => new { r.Id, r.IsSealed })
                .FirstOrDefaultAsync();
            if (role == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(request.RoleId),
                    ValidationErrorCodes.NotFound);
            }

            // NOTE: Ensure we are not changing sealed role
            if (!request.IgnoreSealed && role.IsSealed)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(request.RoleId),
                    ValidationErrorCodes.AccessDenied);
            }

            var permissionExists = await dbContext.Permissions
                .AsNoTracking()
                .AnyAsync(p => p.Id == request.PermissionId);
            if (!permissionExists)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(request.PermissionId),
                    ValidationErrorCodes.NotFound);
            }

            dbContext.RolePermissions.Add(new()
            {
                RoleId = request.RoleId,
                PermissionId = request.PermissionId
            });
            await dbContext.SaveChangesAsync();

            // NOTE: Notify all users which permissions are changed
            var userIds = await dbContext.UserRoles
                .AsNoTracking()
                .Where(ur => ur.RoleId == request.RoleId)
                .Select(ur => ur.UserId)
                .ToListAsync();
            foreach (var userId in userIds)
            {
                await authSessionManager.OnUserChangedAsync(userId);
            }
        }

        public async Task DeletePermissionFromRoleAsync(RolePermissionRequest request)
        {
            var role = await dbContext.Roles
                .AsNoTracking()
                .Where(r => r.Id == request.RoleId)
                .Select(r => new { r.Id, r.IsSealed })
                .FirstOrDefaultAsync();
            if (role == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(request.RoleId),
                    ValidationErrorCodes.NotFound);
            }

            // NOTE: Ensure we are not changing sealed role
            if (!request.IgnoreSealed && role.IsSealed)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(request.RoleId),
                    ValidationErrorCodes.AccessDenied);
            }

            dbContext.RolePermissions.Remove(new()
            {
                RoleId = request.RoleId,
                PermissionId = request.PermissionId
            });
            await dbContext.SaveChangesAsync();

            // NOTE: Notify all users which permissions are changed
            var userIds = await dbContext.UserRoles
                .AsNoTracking()
                .Where(ur => ur.RoleId == request.RoleId)
                .Select(ur => ur.UserId)
                .ToListAsync();
            foreach (var userId in userIds)
            {
                await authSessionManager.OnUserChangedAsync(userId);
            }
        }
        #endregion

        #region Internals
        private async Task<HashSet<string>> GetAllNormilizedRoleNamesAsync()
        {
            var normilizedRoleNames = await dbContext.Roles
                .AsNoTracking()
                .Select(r => r.NormalizedName)
                .ToListAsync();

            return normilizedRoleNames.ToHashSet();
        }
        #endregion
    }
}

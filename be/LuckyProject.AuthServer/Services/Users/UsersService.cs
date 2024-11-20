using LuckyProject.AuthServer.DbLayer;
using LuckyProject.AuthServer.Helpers;
using LuckyProject.AuthServer.Services.Roles.Responses;
using LuckyProject.AuthServer.Services.Sessions;
using LuckyProject.AuthServer.Services.Users.Requests;
using LuckyProject.AuthServer.Services.Users.Responses;
using LuckyProject.AuthServer.Services.Users.Validators;
using LuckyProject.Lib.Basics.Collections;
using LuckyProject.Lib.Basics.Constants;
using LuckyProject.Lib.Basics.Extensions;
using LuckyProject.Lib.Basics.Models;
using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.Web.Constants;
using LuckyProject.Lib.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.AuthServer.Services.Users
{
    public class UsersService : IUsersService
    {
        #region Internals & ctor
        private readonly UserManager<AuthServerUser> userManager;
        private readonly AuthServerDbContext dbContext;
        private readonly IValidationService validationService;
        private readonly ILpAuthorizationService authService;
        private readonly IAuthSessionManager authSessionManager;
        private readonly ILpLocalizationService localizationService;

        public UsersService(
            UserManager<AuthServerUser> userManager,
            SignInManager<AuthServerUser> signInManager,
            AuthServerDbContext dbContext,
            IValidationService validationService,
            IHttpContextAccessor httpContextAccessor,
            ILpAuthorizationService authService,
            IAuthSessionManager authSessionManager,
            ILpLocalizationService localizationService)
        {
            this.userManager = userManager;
            this.dbContext = dbContext;
            this.validationService = validationService;
            this.authService = authService;
            this.authSessionManager = authSessionManager;
            this.localizationService = localizationService;
        }
        #endregion

        #region Helpers
        public string GetMachineUserName(string clientName)
        {
            return $"{LpWebConstants.ApiClients.MachinePrefix}{clientName}";
        }
        #endregion

        #region CRUD
        public async Task<UserResponse> CreateWebUserAsync(CreateWebUserRequest request)
        {
            request.PrepareRequest(userManager.KeyNormalizer);
            var (normalizedUserNames, normalizedEmails, tgUserNames) =
                await GetAllNormalizedDataAsync();
            await validationService.EnsureValidAsync(
                request,
                new CreateWebUserRequestValidator(
                    userManager,
                    normalizedUserNames,
                    normalizedEmails,
                    tgUserNames));
            var ir = await userManager.CreateAsync(
                new AuthServerUser
                {
                    UserName = request.UserName,
                    Email = request.Email,
                    EmailConfirmed = true,
                    PhoneNumber = request.PhoneNumber,
                    TwoFactorEnabled = false,
                    FullName = request.FullName,
                    TelegramUserName = request.TelegramUserName,
                    PreferredLocale = request.PreferredLocale,
                    IsSealed = request.IsSealed
                },
                request.Password);
            ir.EnsureSucceded();
            return await dbContext.Users
                .AsNoTracking()
                .Where(u => u.NormalizedUserName == request.NormalizedUserName)
                .Select(u => new UserResponse
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    NormalizedEmail = u.NormalizedEmail,
                    PhoneNumber = u.PhoneNumber,
                    FullName = u.FullName,
                    TelegramUserName = u.TelegramUserName,
                    PreferredLocale = u.PreferredLocale,
                    PreferredLocaleDisplayName = localizationService
                        .GetLocaleInfo(u.PreferredLocale).DisplayName,
                    MachineClientId = u.MachineClientId,
                    LockoutEndUtc = u.LockoutEnd,
                    IsSealed = u.IsSealed
                })
                .FirstAsync();
        }

        public async Task<UserResponse> CreateMachineUserAsync(CreateMachineUserRequest request)
        {
            var userName = GetMachineUserName(request.ClientName);
            request.NormalizedUserName = userManager.KeyNormalizer.NormalizeName(userName);
            request.NormalizedEmail = userManager.KeyNormalizer.NormalizeEmail(request.Email);
            request.User = new()
            {
                UserName = userName,
                Email = request.Email
            };
            var (normalizedUserNames, normalizedEmails, _) = await GetAllNormalizedDataAsync();
            await validationService.EnsureValidAsync(
                request,
                new CreateMachineUserRequestValidator(
                    userManager,
                    normalizedUserNames,
                    normalizedEmails));
            var ir = await userManager.CreateAsync(
                new AuthServerUser
                {
                    UserName = userName,
                    Email = request.Email,
                    EmailConfirmed = true,
                    PhoneNumber = request.PhoneNumber,
                    TwoFactorEnabled = false,
                    FullName = $"M2M {request.ClientName} User",
                    PreferredLocale = request.PreferredLocale,
                    MachineClientId = request.MachineClientId,
                    IsSealed= request.IsSealed
                });
            ir.EnsureSucceded();
            return await dbContext.Users
                .AsNoTracking()
                .Where(u => u.NormalizedUserName == request.NormalizedUserName)
                .Select(u => new UserResponse
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    NormalizedEmail = u.NormalizedEmail,
                    PhoneNumber = u.PhoneNumber,
                    FullName = u.FullName,
                    PreferredLocale = u.PreferredLocale,
                    PreferredLocaleDisplayName = localizationService
                        .GetLocaleInfo(u.PreferredLocale).DisplayName,
                    MachineClientId = u.MachineClientId,
                    LockoutEndUtc = u.LockoutEnd,
                    IsSealed = u.IsSealed
                })
                .FirstAsync();
        }

        public async Task<PaginatedList<UserResponse>> GetUsersAsync(
            LpFilterOrderPaginationRequest request,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var q = dbContext.Users.AsNoTracking();
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

        public async Task<UserResponse> GetUserByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new UserResponse
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
                .FirstOrDefaultAsync(cancellationToken);
            if (result == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(id),
                    ValidationErrorCodes.NotFound);
            }
            result.PreferredLocaleDisplayName = localizationService
                .GetLocaleInfo(result.PreferredLocale).DisplayName;
            return result;
        }

        public async Task<UserResponse> GetUserByUserNameAsync(
             string userName,
             CancellationToken cancellationToken = default)
        {
            var normilizedUserName = userManager.NormalizeName(userName);
            var result = await dbContext.Users
                .AsNoTracking()
                .Where(u => u.NormalizedUserName == normilizedUserName)
                .Select(u => new UserResponse
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
                .FirstOrDefaultAsync(cancellationToken);
            if (result == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(userName),
                    ValidationErrorCodes.NotFound);
            }

            result.PreferredLocaleDisplayName = localizationService
                .GetLocaleInfo(result.PreferredLocale).DisplayName;
            return result;
        }

        public async Task<UserResponse> GetUserByEmailAsync(
             string email,
             CancellationToken cancellationToken = default)
        {
            var normilizedEmail = userManager.NormalizeEmail(email);
            var result = await dbContext.Users
                .AsNoTracking()
                .Where(u => u.NormalizedEmail == normilizedEmail)
                .Select(u => new UserResponse
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
                .FirstOrDefaultAsync(cancellationToken);
            if (result == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(email),
                    ValidationErrorCodes.NotFound);
            }

            result.PreferredLocaleDisplayName = localizationService
                .GetLocaleInfo(result.PreferredLocale).DisplayName;
            return result;
        }

        public async Task<Guid> GetMachineClientUserId(
            string machineClientId,
            CancellationToken cancellationToken = default)
        {
            return await dbContext.Users
                .AsNoTracking()
                .Where(u => u.MachineClientId == machineClientId)
                .Select(u => u.Id)
                .FirstAsync(cancellationToken);
        }

        public async Task<UserResponse> UpdateWebUserAsync(UpdateWebUserRequest request)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == request.Id);
            if (user == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(request.Id),
                    ValidationErrorCodes.NotFound);
            }

            request.PrepareRequest(userManager.KeyNormalizer);
            var (_, normalizedEmails, tgUserNames) = await GetAllNormalizedDataAsync();
            normalizedEmails = normalizedEmails
                .Except(new List<string>() { user.NormalizedEmail })
                .ToHashSet();
            tgUserNames = tgUserNames
                .Except(new List<string>() { user.TelegramUserName })
                .ToHashSet();
            await validationService.EnsureValidAsync(
                request,
                new UpdateWebUserRequestValidator(normalizedEmails, tgUserNames));

            user.Email = request.Email;
            user.NormalizedEmail = request.NormalizedEmail;
            user.PhoneNumber = request.PhoneNumber;
            user.FullName = request.FullName;
            user.TelegramUserName = request.TelegramUserName;
            user.PreferredLocale = request.PreferredLocale;

            await dbContext.SaveChangesAsync();
            await authSessionManager.OnUserChangedAsync(user.Id);
            return new UserResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                NormalizedEmail = user.NormalizedEmail,
                PhoneNumber = user.PhoneNumber,
                FullName = user.FullName,
                TelegramUserName = user.TelegramUserName,
                PreferredLocale = user.PreferredLocale,
                PreferredLocaleDisplayName = localizationService
                    .GetLocaleInfo(user.PreferredLocale).DisplayName,
                MachineClientId = user.MachineClientId,
                LockoutEndUtc = user.LockoutEnd,
                IsSealed = user.IsSealed
            };
        }

        public async Task<UserResponse> UpdateMachineUserAsync(UpdateMachineUserRequest request)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == request.Id);
            if (user == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(request.Id),
                    ValidationErrorCodes.NotFound);
            }

            request.PrepareRequest(userManager.KeyNormalizer);
            var normalizedEmails = await GetAllNormalizedEmailsAsync();
            normalizedEmails = normalizedEmails
                .Except(new List<string>() { user.NormalizedEmail })
                .ToHashSet();
            await validationService.EnsureValidAsync(
                request,
                new UpdateMachineUserRequestValidator(normalizedEmails));

            user.Email = request.Email;
            user.NormalizedEmail = request.NormalizedEmail;
            user.PhoneNumber = request.PhoneNumber;
            user.PreferredLocale = request.PreferredLocale;

            await dbContext.SaveChangesAsync();
            await authSessionManager.OnUserChangedAsync(user.Id);
            return new UserResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                NormalizedEmail = user.NormalizedEmail,
                PhoneNumber = user.PhoneNumber,
                FullName = user.FullName,
                PreferredLocale = user.PreferredLocale,
                PreferredLocaleDisplayName = localizationService
                    .GetLocaleInfo(user.PreferredLocale).DisplayName,
                MachineClientId = user.MachineClientId,
                LockoutEndUtc = user.LockoutEnd,
                IsSealed = user.IsSealed,
            };
        }

        public async Task DeleteUserAsync(
            Guid id,
            bool ignoreSealed = false,
            bool ignoreMachine = false)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(id),
                    ValidationErrorCodes.NotFound);
            }

            // NOTE: Ensure we are not deleting sealed user
            if (!ignoreSealed && user.IsSealed)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(id),
                    ValidationErrorCodes.AccessDenied);
            }

            // NOTE: Ensure we are not deleting machine user
            if (!ignoreMachine && !string.IsNullOrEmpty(user.MachineClientId))
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(id),
                    ValidationErrorCodes.AccessDenied);
            }

            await userManager.DeleteAsync(user);
            await authSessionManager.OnUserLoggedOutFullOrDeletedAsync(id);
        }
        #endregion

        #region User-Roles management
        public async Task<List<Guid>> GetUserRoleIdsAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await dbContext.UserRoles
                .AsNoTracking()
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToListAsync(cancellationToken);
        }

        public async Task<PaginatedList<RoleResponse>> GetUserRolesAsync(
            Guid userId,
            LpFilterOrderPaginationRequest request,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var user = await dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => new
                {
                    u.Id,
                    u.IsSealed,
                })
                .FirstOrDefaultAsync(cancellationToken);
            if (user == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(userId),
                    ValidationErrorCodes.NotFound);
            }

            var roleIds = await dbContext.UserRoles
                .AsNoTracking()
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToListAsync(cancellationToken);
            var q = dbContext.Roles
                .AsNoTracking()
                .Where(r => roleIds.Contains(r.Id));
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

        public async Task AssignRoleToUserAsync(UserRoleRequest request)
        {
            var user = await dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == request.UserId)
                .Select(u => new { u.Id, u.IsSealed })
                .FirstOrDefaultAsync();
            if (user == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(request.UserId),
                    ValidationErrorCodes.NotFound);
            }

            // NOTE: Ensure we are not changing sealed role
            if (!request.IgnoreSealed && user.IsSealed)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(request.UserId),
                    ValidationErrorCodes.AccessDenied);
            }

            var roleExists = await dbContext.Roles
                .AsNoTracking()
                .AnyAsync(r => r.Id == request.RoleId);
            if (!roleExists)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(request.RoleId),
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

        public async Task DeleteRoleFromUserAsync(UserRoleRequest request)
        {
            var user = await dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == request.UserId)
                .Select(u => new { u.Id, u.IsSealed })
                .FirstOrDefaultAsync();
            if (user == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(request.UserId),
                    ValidationErrorCodes.NotFound);
            }

            // NOTE: Ensure we are not changing sealed role
            if (!request.IgnoreSealed && user.IsSealed)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(request.UserId),
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

        #region User-permissions management
        public async Task<PaginatedList<UserPermissionsResponse>> GetUserPermissionsAsync(
            Guid userId,
            LpFilterOrderPaginationRequest request,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var user = await dbContext.Users
                .AsNoTracking()
                .Where(r => r.Id == userId)
                .Select(r => new
                {
                    r.Id,
                    r.IsSealed,
                })
                .FirstOrDefaultAsync(cancellationToken);
            if (user == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(userId),
                    ValidationErrorCodes.NotFound);
            }

            var userRoleIds = await dbContext.UserRoles
                .AsNoTracking()
                .Where(ur => ur.UserId == userId)
                .OrderBy(ur => ur.RoleId)
                .Select(ur => ur.RoleId)
                .ToListAsync(cancellationToken);

            var userRolesQ = dbContext.Roles
                .AsNoTracking()
                .Where(r => userRoleIds.Contains(r.Id));
            var roleNameFilter = request.GetFilter<string>("role");
            if (!string.IsNullOrEmpty(roleNameFilter))
            {
                userRolesQ = userRolesQ.Where(r => r.Name.Contains(roleNameFilter));
            }
            var userRoles = await userRolesQ
                .Select(r => new { r.Id, r.Name })
                .ToDictionaryAsync(r => r.Id, cancellationToken);
            userRoleIds = userRoles.Keys.ToList();

            var rolePermissions = await dbContext.RolePermissions
                .AsNoTracking()
                .Where(rp => userRoleIds.Contains(rp.RoleId))
                .ToListAsync(cancellationToken);

            var permissionIds = rolePermissions
                .GroupBy(rp => rp.PermissionId)
                .Select(g => g.Key)
                .ToList();
            var q = dbContext.Permissions
                .Include(p => p.BinaryValue)
                .Include(p => p.LevelValue)
                .Include(p => p.PasskeyValue)
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
            var isSealedFilter = request.GetFilter<bool?>("sealed");
            if (isSealedFilter.HasValue)
            {
                q = q.Where(p => p.IsSealed == isSealedFilter.Value);
            }

            var permissions = await q
                .Select(p => new
                {
                    p.Id,
                    p.FullName,
                    p.Description,
                    p.IsSealed,
                    Allow = p.BinaryValue != null ? p.BinaryValue.Allow : (bool?)null,
                    Level = p.LevelValue != null ? p.LevelValue.Level : (int?)null,
                    Passkeys = p.PasskeyValue != null ? p.PasskeyValue.Select(v => v.Passkey).ToHashSet() : null
                })
                .ToDictionaryAsync(p => p.Id, p => p, cancellationToken);

            var userPermissions = new List<UserPermissionsResponse>();
            var permissionIdsByRoleIds = rolePermissions
                .GroupBy(rp => rp.RoleId)
                .ToDictionary(g => g.Key, g => g.Select(rp => rp.PermissionId).ToList());
            foreach (var roleId in userRoleIds)
            {
                var role = userRoles[roleId];
                if (!permissionIdsByRoleIds.TryGetValue(roleId, out var permissionIdsForRole))
                {
                    continue;
                }

                userPermissions.AddRange(permissionIdsForRole
                    .Select(pid =>
                    {
                        if (!permissions.TryGetValue(pid, out var p))
                        {
                            return null;
                        }
                        var up = new UserPermissionsResponse
                        {
                            Id = p.Id,
                            FullName = p.FullName,
                            Description = p.Description,
                            IsSealed = p.IsSealed,
                            FromRoleId = role.Id,
                            FromRoleName = role.Name,
                            Allow = p.Allow,
                            Level = p.Level,
                            Passkeys = p.Passkeys
                        };
                        up.Fullfill(authService);
                        return up;
                    }).Where(up => up != null));
            }

            var e = userPermissions.AsEnumerable();
            if (request.Order == "id-asc")
            {
                e = e.OrderBy(p => p.Id);
            }
            else if (request.Order == "id-desc")
            {
                e = e.OrderByDescending(p => p.Id);
            }
            else if (request.Order == "type-asc")
            {
                e = e.OrderBy(p => p.FullName.Substring(0, 1));
            }
            else if (request.Order == "type-desc")
            {
                e = e.OrderByDescending(p => p.FullName.Substring(0, 1));
            }
            else if (request.Order == "name-asc")
            {
                e = e.OrderBy(p => p.FullName.Substring(1));
            }
            else if (request.Order == "name-desc")
            {
                e = e.OrderByDescending(p => p.FullName.Substring(1));
            }
            else if (request.Order == "role-asc")
            {
                e = e.OrderBy(p => p.FromRoleName);
            }
            else if (request.Order == "role-desc")
            {
                e = e.OrderByDescending(p => p.FromRoleName);
            }
            else
            {
                e = e.OrderBy(p => p.Id);
            }

            return e.ToPaginatedList(pageSize, request.Page, false);
        }

        public async Task<AuthorizeResponse> AuthorizeAsync(
            List<LpAuthRequirement> requirements,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var user = await GetUserByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                return null;
            }

            var userRoleIds = await dbContext.UserRoles
                .AsNoTracking()
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToListAsync(cancellationToken);
    
            var permissionIds = await dbContext.RolePermissions
                .AsNoTracking()
                .Where(rp => userRoleIds.Contains(rp.RoleId))
                .GroupBy(rp => rp.PermissionId)
                .Select(g => g.Key)
                .ToListAsync(cancellationToken);

            var permissions = await dbContext.Permissions
                .AsNoTracking()
                .Include(p => p.BinaryValue)
                .Include(p => p.LevelValue)
                .Include(p => p.PasskeyValue)
                .Where(p => permissionIds.Contains(p.Id))
                .Select(p => new { p.FullName, p.BinaryValue, p.LevelValue, p.PasskeyValue })
                .ToListAsync(cancellationToken);

            var lpPermissions = permissions.Select(p =>
            {
                object actualValue = null;
                if (p.BinaryValue != null)
                {
                    actualValue = p.BinaryValue.Allow;
                }
                else if (p.LevelValue != null)
                {
                    actualValue = p.LevelValue.Level;
                }
                else if (p.PasskeyValue.Count > 0)
                {
                    actualValue = p.PasskeyValue.Select(pkv => pkv.Passkey).ToHashSet();
                }

                return new LpAuthPermission { FullName = p.FullName, ActualValue = actualValue };
            }).ToList();

            return authService.HasAccess(requirements, lpPermissions)
                ? new AuthorizeResponse { User = user, UserRoleIds = userRoleIds }
                : null;
        }
        #endregion

        #region Internals
        private async Task<(HashSet<string>, HashSet<string>, HashSet<string>)>
            GetAllNormalizedDataAsync()
        {
            var normilizedData = await dbContext.Users
                .AsNoTracking()
                .Select(u => new { u.NormalizedUserName, u.NormalizedEmail, u.TelegramUserName })
                .ToListAsync();

            return (normilizedData.Select(d => d.NormalizedUserName).ToHashSet(),
                normilizedData.Select(d => d.NormalizedEmail).ToHashSet(),
                normilizedData.Select(d => d.TelegramUserName).ToHashSet());
        }

        private async Task<HashSet<string>> GetAllNormalizedEmailsAsync()
        {
            return (await dbContext.Users
                .AsNoTracking()
                .Select(u => u.NormalizedEmail)
                .ToListAsync())
                .ToHashSet();
        }
        #endregion
    }
}

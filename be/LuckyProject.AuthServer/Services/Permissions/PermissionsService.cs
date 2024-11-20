using LuckyProject.AuthServer.DbLayer;
using LuckyProject.AuthServer.Helpers;
using LuckyProject.AuthServer.Services.Permissions.Requests;
using LuckyProject.AuthServer.Services.Permissions.Responses;
using LuckyProject.AuthServer.Services.Permissions.Validators;
using LuckyProject.Lib.Basics.Collections;
using LuckyProject.Lib.Basics.Constants;
using LuckyProject.Lib.Basics.Extensions;
using LuckyProject.Lib.Basics.Models;
using LuckyProject.Lib.Basics.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.AuthServer.Services.Permissions
{
    public class PermissionsService : IPermissionsService
    {
        #region Internals & ctor
        private readonly AuthServerDbContext dbContext;
        private readonly IValidationService validationService;
        private readonly ILpAuthorizationService authService;

        public PermissionsService(
            AuthServerDbContext dbContext,
            IValidationService validationService,
            ILpAuthorizationService authService)
        {
            this.dbContext = dbContext;
            this.validationService = validationService;
            this.authService = authService;
        }
        #endregion

        #region CRUD
        public async Task<PermissionResponse> CreatePermissionAsync(CreatePermissionRequest request)
        {
            request.PrepareRequest(authService);
            var permissionFullNames = await GetAllPermissionFullNamesAsync();
            validationService.EnsureValid(
                request,
                new CreatePermissionRequestValidator(permissionFullNames));

            var permission = new AuthServerPermission()
            {
                FullName = request.FullName,
                Description = request.Description,
                IsSealed = request.IsSealed,
                ApiId = request.ApiId
            };

            if (request.Type == LpAuthPermissionType.Binary)
            {
                permission.BinaryValue = new()
                {
                    Allow = request.Allow.Value,
                    Permission = permission
                };
            }
            else if (request.Type == LpAuthPermissionType.Level)
            {
                permission.LevelValue = new()
                {
                    Level = request.Level.Value,
                    Permission = permission
                };
            }
            else if (request.Type == LpAuthPermissionType.Passkey)
            {
                permission.PasskeyValue = request.Passkeys
                    .Select(pk => new AuthServerPasskeyPermissionValue
                    {
                        Passkey = pk,
                        Permission = permission
                    })
                    .ToHashSet();
            }

            permission = dbContext.Permissions.Add(permission).Entity;
            await dbContext.SaveChangesAsync();
            return ToPermissionResponse(permission);
        }

        public async Task<PaginatedList<PermissionResponse>> GetPermissionsAsync(
            LpFilterOrderPaginationRequest request,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var q = dbContext.Permissions
                .Include(p => p.BinaryValue)
                .Include(p => p.LevelValue)
                .Include(p => p.PasskeyValue)
                .AsNoTracking();
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

            var permissions = await q.ToPaginatedListAsync(
                pageSize,
                request.Page,
                false,
                cancellationToken);
            return permissions.Items
                .Select(ToPermissionResponse)
                .ToPaginatedList(permissions.Pagination);
        }

        public async Task<PermissionResponse> GetPermissionByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await dbContext.Permissions
                .Include(p => p.BinaryValue)
                .Include(p => p.LevelValue)
                .Include(p => p.PasskeyValue)
                .AsNoTracking()
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
            if (result == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(id),
                    ValidationErrorCodes.NotFound);
            }

            return ToPermissionResponse(result);
        }

        public async Task<PermissionResponse> GetPermissionByFullNameAsync(
            string fullName,
            CancellationToken cancellationToken = default)
        {
            var result = await dbContext.Permissions
                .Include(p => p.BinaryValue)
                .Include(p => p.LevelValue)
                .Include(p => p.PasskeyValue)
                .AsNoTracking()
                .Where(p => p.FullName == fullName)
                .FirstOrDefaultAsync(cancellationToken);
            if (result == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(fullName),
                    ValidationErrorCodes.NotFound);
            }

            return ToPermissionResponse(result);
        }

        public async Task<PermissionResponse> UpdatePermissionAsync(UpdatePermissionRequest request)
        {
            var permission = await dbContext.Permissions
                .Include(p => p.BinaryValue)
                .Include(p => p.LevelValue)
                .Include(p => p.PasskeyValue)
                .FirstOrDefaultAsync(p => p.Id == request.Id);
            if (permission == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(request.Id),
                    ValidationErrorCodes.NotFound);
            }

            // NOTE: Ensure we are not changing sealed permission
            if (!request.IgnoreSealed && permission.IsSealed)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(request.Id),
                    ValidationErrorCodes.AccessDenied);
            }


            request.PrepareRequest(authService, permission.FullName);
            validationService.EnsureValid(
                request,
                new UpdatePermissionRequestValidator());

            permission.Description = request.Description;
            if (request.Type == LpAuthPermissionType.Binary)
            {
                permission.BinaryValue.Allow = request.Allow.Value;
            }
            else if (request.Type == LpAuthPermissionType.Level)
            {
                permission.LevelValue.Level = request.Level.Value;
            }
            else if (request.Type == LpAuthPermissionType.Passkey)
            {
                dbContext.PasskeyPermissionsValue.RemoveRange(permission.PasskeyValue);
                permission.PasskeyValue = request.Passkeys
                    .Select(pk => new AuthServerPasskeyPermissionValue
                    {
                        Passkey = pk,
                        Permission = permission
                    })
                    .ToHashSet();
            }

            await dbContext.SaveChangesAsync();
            return ToPermissionResponse(permission);
        }

        public async Task DeletePermissionAsync(Guid id, bool ignoreSealed = false)
        {
            var permission = await dbContext.Permissions
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new { p.Id, p.IsSealed })
                .FirstOrDefaultAsync();
            if (permission == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(id),
                    ValidationErrorCodes.NotFound);
            }

            // NOTE: Ensure we are not changing sealed permission
            if (!ignoreSealed && permission.IsSealed)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(id),
                    ValidationErrorCodes.AccessDenied);
            }

            dbContext.Permissions.Remove(new AuthServerPermission { Id = permission.Id });
            await dbContext.SaveChangesAsync();
        }
        #endregion

        #region Internals
        private PermissionResponse ToPermissionResponse(AuthServerPermission p)
        {
            var passkeys = p.PasskeyValue.Select(c => c.Passkey).ToHashSet();
            return new PermissionResponse
            {
                Id = p.Id,
                Type = authService.GetPermissionType(p.FullName),
                Name = authService.GetPermissionName(p.FullName),
                FullName = p.FullName,
                Description = p.Description,
                IsSealed = p.IsSealed,
                Allow = p.BinaryValue?.Allow,
                Level = p.LevelValue?.Level,
                Passkeys = passkeys.Count > 0 ? passkeys : null,
                ApiId = p.ApiId
            };
        }

        private async Task<HashSet<string>> GetAllPermissionFullNamesAsync()
        {
            return (await dbContext.Permissions
                .AsNoTracking()
                .Select(u => u.FullName)
                .ToListAsync())
                .ToHashSet();
        }
        #endregion
    }
}

using LuckyProject.AuthServer.DbLayer;
using LuckyProject.AuthServer.Helpers;
using LuckyProject.AuthServer.Services.Users.Requests;
using LuckyProject.AuthServer.Services.Users.Responses;
using LuckyProject.Lib.Basics.Constants;
using LuckyProject.Lib.Basics.LiveObjects;
using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.Telegram.LiveObjects;
using LuckyProject.Lib.Telegram.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuckyProject.AuthServer.Services.Users
{
    public class UsersLoginServiceSecrets
    {
        public string AuthenticatorTgBotToken { get; set; }
    }

    public class UsersLoginService : IUsersLoginService
    {
        #region Internals & ctor
        private const string TgBotClientStartMessage = "/start";
        private const int TwoFactorMaxTryCount = 3;
        private static readonly TimeSpan TwoFactorExpireTimeout = TimeSpan.FromMinutes(3);

        private const int ResetPasswordMaxTryCount = 3;
        private static readonly TimeSpan ResetPasswordExpireTimeout = TimeSpan.FromMinutes(3);

        private class TwoFactorCacheEntry
        {
            public Guid UserId { get; init; }
            public string Password { get; init; }
            public string ExpectedCode { get; set; }
            public int RequestCount { get; set; }
        }

        private class ResetPasswordCacheEntry
        {
            public Guid UserId { get; init; }
            public string ExpectedCode { get; set; }
            public int RequestCount { get; set; }
        }

        private class TgBotSubscriber
        {
            public Telegram.Bot.Types.ChatId ChatId { get; }
            public string UserName { get; }

            public TgBotSubscriber(Telegram.Bot.Types.ChatId chatId, string userName)
            {
                ChatId = chatId;
                UserName = userName;
            }

            public override bool Equals(object obj)
            {
                return obj is TgBotSubscriber subscriber && UserName == subscriber.UserName;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(UserName);
            }
        }

        private readonly UsersLoginServiceSecrets secrets;

        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILpTelegramBotClientFactory tgBotFactory;
        private readonly IStringService stringService;
        private readonly ILpLocalizationService localizationService;
        private readonly IAppVersionService appVersionService;

        private readonly ILpInMemoryCache<Guid, TwoFactorCacheEntry> twoFactorCache;
        private readonly ILpInMemoryCache<Guid, ResetPasswordCacheEntry> resetPasswordCache;

        private ILpTelegramBotClient tgBotClient;
        private readonly HashSet<TgBotSubscriber> tgBotSubscribers = new();

        public UsersLoginService(
            IOptions<UsersLoginServiceSecrets> secrets,
            IServiceScopeFactory serviceScopeFactory,
            IHttpContextAccessor httpContextAccessor,
            ILpTelegramBotClientFactory tgBotFactory,
            IStringService stringService,
            ILpLocalizationService localizationService,
            IAppVersionService appVersionService,
            ILpInMemoryCacheFactory cacheFactory)
        {
            this.secrets = secrets.Value;
            this.httpContextAccessor = httpContextAccessor;
            this.serviceScopeFactory = serviceScopeFactory;
            this.tgBotFactory = tgBotFactory;
            this.stringService = stringService;
            this.localizationService = localizationService;
            this.appVersionService = appVersionService;

            twoFactorCache = cacheFactory.CreateCache<Guid, TwoFactorCacheEntry>(
                TwoFactorExpireTimeout,
                null,
                TwoFactorExpireTimeout / 10);
            resetPasswordCache = cacheFactory.CreateCache<Guid, ResetPasswordCacheEntry>(
                ResetPasswordExpireTimeout,
                null,
                ResetPasswordExpireTimeout / 10);
        }
        #endregion

        #region Public interface
        public void Start()
        {
            tgBotClient = tgBotFactory.CreateClient(
                secrets.AuthenticatorTgBotToken,
                "Authenticator");
            tgBotClient.OnMessage += TgBotClient_OnMessage;
        }

        public async Task StopAsync()
        {
            foreach (var s in tgBotSubscribers)
            {
                var user = await GetUserByTelegramUserNameAsync(s.UserName);
                var stopMessage = localizationService.GetLocalizedString(
                    user.PreferredLocale,
                    "lp.authserver.be.services.userLogin.tgBotStopping",
                    "Lucky Project Authenticator Bot is stopped.\r\n\r\n" +
                    "Do not forget to send /start command after bot is restarted");
                await tgBotClient.SendTextMessageAsync(
                    chatId: s.ChatId,
                    text: stopMessage,
                    protectContent: true);
            }
            tgBotClient.OnMessage -= TgBotClient_OnMessage;
            tgBotClient.Dispose();
            tgBotSubscribers.Clear();
            twoFactorCache.Clear();
        }

        public async Task<bool> CanLoginAsync(Guid id)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<AuthServerUser>>();
            var signInManager = scope.ServiceProvider
                .GetRequiredService<SignInManager<AuthServerUser>>();

            var user = await userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return false;
            }

            return await signInManager.CanSignInAsync(user) &&
                !await userManager.IsLockedOutAsync(user);
        }

        public async Task<bool> CheckPasswordAsync(CheckPasswordRequest request)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<AuthServerUser>>();
            var signInManager = scope.ServiceProvider
                .GetRequiredService<SignInManager<AuthServerUser>>();

            var user = await userManager.FindByIdAsync(request.Id.ToString());
            if (user == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(request.Id),
                    ValidationErrorCodes.NotFound);
            }

            if (string.IsNullOrEmpty(request.Password))
            {
                return false;
            }

            var result = await signInManager.CheckPasswordSignInAsync(
                user,
                request.Password,
                false);
            return result.Succeeded;
        }

        public async Task<VerifyPasswordResponse> VerifyPasswordAsync(
            string userNameOrEmail,
            string password)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<AuthServerUser>>();
            var signInManager = scope.ServiceProvider
                .GetRequiredService<SignInManager<AuthServerUser>>();

            var user = userNameOrEmail.Contains("@")
                ? await userManager.FindByEmailAsync(userNameOrEmail)
                : await userManager.FindByNameAsync(userNameOrEmail);
            if (user == null)
            {
                return new VerifyPasswordResponse { CredsValid = false };
            }

            if (!string.IsNullOrEmpty(user.MachineClientId))
            {
                return new VerifyPasswordResponse { CredsValid = false };
            }
            
            if (user.LockoutEnabled && user.LockoutEnd >= DateTimeOffset.UtcNow)
            {
                return new VerifyPasswordResponse { IsLockedOut = true, CredsValid = false };
            }

            var checkPasswordResult = await signInManager.CheckPasswordSignInAsync(
                user,
                password,
                true);
            if (!checkPasswordResult.Succeeded || checkPasswordResult.IsLockedOut)
            {
                return new VerifyPasswordResponse
                {
                    IsLockedOut = checkPasswordResult.IsLockedOut,
                    CredsValid = false
                };
            }

            var twoFactorExpectedCode = stringService.GenerateRandomString("0123456789", 6);
            if (!await SendTwoFactorCodeAsync(
                user.TelegramUserName,
                user.FullName,
                user.PreferredLocale,
                twoFactorExpectedCode))
            {
                return new VerifyPasswordResponse
                {
                    IsLockedOut = false,
                    CredsValid = true
                };
            }

            var twoFactorRequestId = Guid.NewGuid();
            twoFactorCache.Add(twoFactorRequestId, new()
            {
                UserId = user.Id,
                Password = password,
                RequestCount = 1,
                ExpectedCode = twoFactorExpectedCode,
            });
            return new VerifyPasswordResponse
            {
                User = new UserResponse
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    NormalizedEmail = user.NormalizedEmail,
                    PhoneNumber = user.PhoneNumber,
                    FullName = user.FullName,
                    TelegramUserName = user.TelegramUserName,
                    PreferredLocale = user.PreferredLocale,
                    MachineClientId = user.MachineClientId,
                    LockoutEndUtc = user.LockoutEnd
                },
                IsLockedOut = false,
                CredsValid = true,
                TwoFactorRequestId = twoFactorRequestId
            };
        }

        public async Task<bool> RequestTwoFactorCodeAgainAsync(Guid twoFactorRequestId)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<AuthServerUser>>();

            if (!twoFactorCache.Contains(twoFactorRequestId))
            {
                return false;
            }
            var twoFactorCacheEntry = twoFactorCache.Get(twoFactorRequestId);
            if (twoFactorCacheEntry.RequestCount++ >= TwoFactorMaxTryCount)
            {
                twoFactorCache.Delete(twoFactorRequestId);
                return false;
            }

            var user = await userManager.FindByIdAsync(twoFactorCacheEntry.UserId.ToString());
            if (user == null)
            {
                twoFactorCache.Delete(twoFactorRequestId);
                return false;
            }

            var expectedCode = stringService.GenerateRandomString("0123456789", 6);
            if (!await SendTwoFactorCodeAsync(
                user.TelegramUserName,
                user.FullName,
                user.PreferredLocale,
                expectedCode))
            {
                return false;
            }

            twoFactorCacheEntry.ExpectedCode = expectedCode;
            return true;
        }

        public async Task<LoginResponse> LoginAsync(
            Guid twoFactorRequestId,
            string twoFactorCode)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<AuthServerUser>>();
            var signInManager = scope.ServiceProvider
                .GetRequiredService<SignInManager<AuthServerUser>>();

            if (string.IsNullOrEmpty(twoFactorCode))
            {
                return new LoginResponse { Success = false };
            }

            if (!twoFactorCache.Contains(twoFactorRequestId))
            {
                return new LoginResponse { Success = false };
            }
            var twoFactorCacheEntry = twoFactorCache.Get(twoFactorRequestId);

            var user = await userManager.FindByIdAsync(twoFactorCacheEntry.UserId.ToString());
            if (user == null)
            {
                return new LoginResponse { Success = false };
            }

            if (user.LockoutEnabled && user.LockoutEnd >= DateTimeOffset.UtcNow)
            {
                return new LoginResponse { IsLockedOut = true, Success = false };
            }

            if (!twoFactorCacheEntry.ExpectedCode.Equals(twoFactorCode, StringComparison.Ordinal))
            {
                await userManager.AccessFailedAsync(user);
                return new LoginResponse
                {
                    IsLockedOut = await userManager.IsLockedOutAsync(user),
                    Success = false
                };
            }

            twoFactorCache.Delete(twoFactorRequestId);

            var result = await signInManager.PasswordSignInAsync(
                user,
                twoFactorCacheEntry.Password,
                true,
                true);
            EnsureIdentityCookiesDeleted();
            if (!result.Succeeded)
            {
                return new LoginResponse
                {
                    IsLockedOut = result.IsLockedOut,
                    Success = false
                };
            }

            return new LoginResponse
            {
                User = new UserResponse
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    NormalizedEmail = user.NormalizedEmail,
                    PhoneNumber = user.PhoneNumber,
                    FullName = user.FullName,
                    TelegramUserName = user.TelegramUserName,
                    PreferredLocale = user.PreferredLocale,
                    MachineClientId = user.MachineClientId,
                    LockoutEndUtc = user.LockoutEnd
                },
                IsLockedOut = false,
                Success = true
            };
        }

        public async Task LogoutAsync()
        {
            using var scope = serviceScopeFactory.CreateScope();
            var signInManager = scope.ServiceProvider
                .GetRequiredService<SignInManager<AuthServerUser>>();
            await signInManager.SignOutAsync();
            EnsureIdentityCookiesDeleted();
        }

        public async Task<ForgotPasswordResponse> ForgotPasswordAsync(string userNameOrEmail)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<AuthServerUser>>();
            var user = userNameOrEmail.Contains("@")
                ? await userManager.FindByEmailAsync(userNameOrEmail)
                : await userManager.FindByNameAsync(userNameOrEmail);
            if (user == null)
            {
                return new ForgotPasswordResponse { UserFound = false };
            }

            if (!string.IsNullOrEmpty(user.MachineClientId))
            {
                return new ForgotPasswordResponse { UserFound = false };
            }

            if (user.LockoutEnabled && user.LockoutEnd >= DateTimeOffset.UtcNow)
            {
                return new ForgotPasswordResponse { IsLockedOut = true, UserFound = true };
            }

            var resetPasswordExpectedCode = stringService.GenerateRandomString("0123456789", 6);
            if (!await SendResetPasswordCodeAsync(
                user.TelegramUserName,
                user.FullName,
                user.PreferredLocale,
                resetPasswordExpectedCode))
            {
                return new ForgotPasswordResponse
                {
                    IsLockedOut = false,
                    UserFound = true
                };
            }

            var resetPasswordRequestId = Guid.NewGuid();
            resetPasswordCache.Add(resetPasswordRequestId, new()
            {
                UserId = user.Id,
                RequestCount = 1,
                ExpectedCode = resetPasswordExpectedCode,
            });
            return new ForgotPasswordResponse
            {
                User = new UserResponse
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    NormalizedEmail = user.NormalizedEmail,
                    PhoneNumber = user.PhoneNumber,
                    FullName = user.FullName,
                    TelegramUserName = user.TelegramUserName,
                    PreferredLocale = user.PreferredLocale,
                    MachineClientId = user.MachineClientId,
                    LockoutEndUtc = user.LockoutEnd
                },
                IsLockedOut = false,
                UserFound = true,
                RequestId = resetPasswordRequestId
            };
        }

        public async Task<bool> RequestResetPasswordCodeAgainAsync(Guid resetPasswordRequestId)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<AuthServerUser>>();

            if (!resetPasswordCache.Contains(resetPasswordRequestId))
            {
                return false;
            }
            var resetPasswordCacheEntry = resetPasswordCache.Get(resetPasswordRequestId);
            if (resetPasswordCacheEntry.RequestCount++ >= TwoFactorMaxTryCount)
            {
                resetPasswordCache.Delete(resetPasswordRequestId);
                return false;
            }

            var user = await userManager.FindByIdAsync(resetPasswordCacheEntry.UserId.ToString());
            if (user == null)
            {
                resetPasswordCache.Delete(resetPasswordRequestId);
                return false;
            }

            var expectedCode = stringService.GenerateRandomString("0123456789", 6);
            if (!await SendResetPasswordCodeAsync(
                user.TelegramUserName,
                user.FullName,
                user.PreferredLocale,
                expectedCode))
            {
                return false;
            }

            resetPasswordCacheEntry.ExpectedCode = expectedCode;
            return true;
        }

        public async Task<ResetPasswordResponse> ResetPasswordAsync(
            Guid resetPasswordRequestId,
            string newPassword,
            string newPasswordRepeat,
            string code)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<AuthServerUser>>();

            if (string.IsNullOrEmpty(code))
            {
                return ResetPasswordResponse.InvalidCode;
            }

            if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(newPasswordRepeat))
            {
                return ResetPasswordResponse.InvalidPassword;
            }

            if (newPassword != newPasswordRepeat)
            {
                return ResetPasswordResponse.PasswordsMistmatch;
            }

            if (!resetPasswordCache.Contains(resetPasswordRequestId))
            {
                return ResetPasswordResponse.NotRequested;
            }
            var resetPasswordCacheEntry = resetPasswordCache.Get(resetPasswordRequestId);

            var user = await userManager.FindByIdAsync(resetPasswordCacheEntry.UserId.ToString());
            if (user == null)
            {
                return ResetPasswordResponse.NotRequested;
            }

            if (user.LockoutEnabled && user.LockoutEnd >= DateTimeOffset.UtcNow)
            {
                return ResetPasswordResponse.UserLockedOut;
            }

            foreach (var v in userManager.PasswordValidators)
            {
                var ir = await v.ValidateAsync(userManager, user, newPassword);
                if (!ir.Succeeded)
                {
                    return ResetPasswordResponse.InvalidPassword;
                }
            }

            if (!resetPasswordCacheEntry.ExpectedCode.Equals(code, StringComparison.Ordinal))
            {
                await userManager.AccessFailedAsync(user);
                var isLockedOut = await userManager.IsLockedOutAsync(user);
                return isLockedOut
                    ? ResetPasswordResponse.UserLockedOut
                    : ResetPasswordResponse.InvalidCode;
            }
            resetPasswordCache.Delete(resetPasswordRequestId);

            var success = await ResetPasswordAsync(userManager, user, newPassword);
            return success ? ResetPasswordResponse.Success : ResetPasswordResponse.Unknown;
        }

        public async Task ResetPasswordAsync(ResetPasswordRequest request)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<AuthServerUser>>();

            var user = await userManager.FindByIdAsync(request.Id.ToString());
            if (user == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(request.Id),
                    ValidationErrorCodes.NotFound);
            }

            if (string.IsNullOrEmpty(request.NewPassword) ||
                string.IsNullOrEmpty(request.NewPasswordRepeat))
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(request.NewPassword),
                    ValidationErrorCodes.Password);
            }

            if (request.NewPassword != request.NewPasswordRepeat)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(request.NewPassword),
                    ValidationErrorCodes.Password);
            }

            foreach (var v in userManager.PasswordValidators)
            {
                var ir = await v.ValidateAsync(userManager, user, request.NewPassword);
                if (!ir.Succeeded)
                {
                    AuthServerThrowHelper.ThrowValidationErrorException(
                        nameof(request.NewPassword),
                        ValidationErrorCodes.Password);
                }
            }

            if (!await ResetPasswordAsync(userManager, user, request.NewPassword))
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(request.NewPassword),
                    ValidationErrorCodes.Unknown);
            }
        }

        private async Task<bool> ResetPasswordAsync(
            UserManager<AuthServerUser> userManager,
            AuthServerUser user,
            string newPassword)
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var result = await userManager.ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded;
        }
        #endregion

        #region Internals
        private async Task TgBotClient_OnMessage(
            Telegram.Bot.Types.Message message,
            Telegram.Bot.Types.Enums.UpdateType type)
        {
            if (message.Text != TgBotClientStartMessage)
            {
                return;
            }

            var tgUserName = $"@{message.Chat.Username}";
            await tgBotClient.SendTextMessageAsync(
                chatId: message.Chat,
                text: $"<code>Lucky Project Authenticator Bot\r\nVersion: {appVersionService.AppVersion}</code>",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                protectContent: true);
            var user = await GetUserByTelegramUserNameAsync(tgUserName);
            if (user == null)
            {
                await tgBotClient.SendTextMessageAsync(
                    chatId: message.Chat,
                    text: "You are not registered as Lucky Project User",
                    protectContent: true);
                return;
            }

            tgBotSubscribers.Add(new(message.Chat.Id, tgUserName));
            var subscribedMessage = localizationService.GetLocalizedString(
                user.PreferredLocale,
                "lp.authserver.be.services.userLogin.tgBotSubscribed",
                "Hello, <b>{0}</b>!\r\n\r\n" +
                "Now you are subscribed to the Lucky Project Authenticator Bot");
            await tgBotClient.SendTextMessageAsync(
                chatId: message.Chat,
                text: string.Format(subscribedMessage, user.FullName),
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                protectContent: true);
        }

        private void EnsureIdentityCookiesDeleted()
        {
            var response = httpContextAccessor.HttpContext.Response;
            response.Cookies.Delete(".AspNetCore.Identity.Application");
            response.Headers.Remove("Set-Cookie");
        }

        private async Task<UserResponse> GetUserByTelegramUserNameAsync(string tgUserName)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AuthServerDbContext>();
            var result = await dbContext.Users
                .AsNoTracking()
                .Where(u => u.TelegramUserName == tgUserName)
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
                    LockoutEndUtc = u.LockoutEnd
                })
                .FirstOrDefaultAsync();
            return result;
        }

        private async Task<bool> SendTwoFactorCodeAsync(
            string tgUserName,
            string fullName,
            string preferredLocale,
            string code)
        {
            if (!tgBotSubscribers.TryGetValue(new(0, tgUserName), out var subscriber))
            {
                return false;
            }

            var message = localizationService.GetLocalizedString(
                preferredLocale,
                "lp.authserver.be.services.userLogin.twoFactorCode",
                "Hello, <b>{0}</b>!\r\n\r\nSomeone tries to Sign In to the Lucky Project with " +
                "your credentials. If it is not you - ignore this message.\r\n\r\n" +
                "Authentication code is <b>{1}</b> - enter this code in the Sign In form");
            await tgBotClient.SendTextMessageAsync(
                chatId: subscriber.ChatId,
                text: string.Format(message, fullName, code),
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                protectContent: true);
            return true;
        }

        private async Task<bool> SendResetPasswordCodeAsync(
            string tgUserName,
            string fullName,
            string preferredLocale,
            string code)
        {
            if (!tgBotSubscribers.TryGetValue(new(0, tgUserName), out var subscriber))
            {
                return false;
            }

            var message = localizationService.GetLocalizedString(
                preferredLocale,
                "lp.authserver.be.services.userLogin.resetPasswordCode",
                "Hello, <b>{0}</b>!\r\n\r\nSomeone tries to Reset Password of your Lucky Project " +
                "User. If it is not you - ignore this message.\r\n\r\n" +
                "Verification code is <b>{1}</b> - enter this code in the Reset Password form");
            await tgBotClient.SendTextMessageAsync(
                chatId: subscriber.ChatId,
                text: string.Format(message, fullName, code),
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                protectContent: true);
            return true;
        }
        #endregion
    }
}

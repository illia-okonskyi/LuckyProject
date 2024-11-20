using LuckyProject.AuthServer.Services.Users.Responses;
using System.Threading.Tasks;
using System;
using LuckyProject.AuthServer.Services.Users.Requests;

namespace LuckyProject.AuthServer.Services.Users
{
    public interface IUsersLoginService
    {
        void Start();
        Task StopAsync();

        Task<bool> CanLoginAsync(Guid id);
        Task<bool> CheckPasswordAsync(CheckPasswordRequest request);
        Task<VerifyPasswordResponse> VerifyPasswordAsync(string userNameOrEmail, string password);
        Task<bool> RequestTwoFactorCodeAgainAsync(Guid twoFactorRequestId);
        Task<LoginResponse> LoginAsync(
            Guid twoFactorRequestId,
            string twoFactorCode);
        Task LogoutAsync();

        Task<ForgotPasswordResponse> ForgotPasswordAsync(string userNameOrEmail);
        Task<bool> RequestResetPasswordCodeAgainAsync(Guid resetPasswordRequestId);
        Task<ResetPasswordResponse> ResetPasswordAsync(
            Guid resetPasswordRequestId,
            string newPassword,
            string newPasswordRepeat,
            string code);
        Task ResetPasswordAsync(ResetPasswordRequest request);
    }
}

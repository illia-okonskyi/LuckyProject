namespace LuckyProject.AuthServer.Services.Users.Responses
{
    public enum ResetPasswordResponse
    {
        NotRequested,
        InvalidCode,
        PasswordsMistmatch,
        InvalidPassword,
        UserLockedOut,
        Unknown,
        Success
    }
}

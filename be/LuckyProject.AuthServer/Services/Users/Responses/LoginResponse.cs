namespace LuckyProject.AuthServer.Services.Users.Responses
{
    public class LoginResponse
    {
        public UserResponse User { get; init; }
        public bool IsLockedOut { get; init; }
        public bool Success { get; init; }
    }
}

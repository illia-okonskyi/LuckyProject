using LuckyProject.AuthServer.DbLayer;

namespace LuckyProject.AuthServer.Services.Users.Requests
{
    public class CreateMachineUserRequest
    {
        public string ClientName { get; init; }
        public string Email { get; init; }
        public string PhoneNumber { get; init; }
        public string PreferredLocale { get; init; }
        public string MachineClientId { get; init; }
        public bool IsSealed { get; init; }

        #region Internal Service usage
        public string NormalizedUserName { get; set; }
        public string NormalizedEmail { get; set; }
        public AuthServerUser User { get; set; }
        #endregion
    }
}

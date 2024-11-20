namespace LuckyProject.AuthServer.Models
{
    public class AuthorizeResultNotification : Notification<bool>
    {
        public AuthorizeResultNotification(bool authorized)
            : base("lp.common.authorizeResult", authorized)
        { }
    }

    public class UserChangedNotification : Notification
    {
        public UserChangedNotification()
            : base("lp.common.userChanged")
        { }
    }
}

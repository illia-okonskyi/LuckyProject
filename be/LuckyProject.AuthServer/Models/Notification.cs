namespace LuckyProject.AuthServer.Models
{
    public class Notification
    {
        public string Type { get; }
        public Notification(string type)
        {
            Type = type;
        }
    }

    public class Notification<TPayload> : Notification
    {
        public TPayload Payload { get; }

        public Notification(string type, TPayload payload)
            : base(type)
        {
            Payload = payload;
        }
    }
}

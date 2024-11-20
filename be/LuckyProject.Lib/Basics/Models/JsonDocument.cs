namespace LuckyProject.Lib.Basics.Models
{
    public class JsonDocument
    {
        public string DocType { get; init; }
        public int DocVersion { get; init; }
    }

    public class JsonDocument<TDocument> : JsonDocument
    {
        public TDocument Document { get; init; }
    }
}

namespace LuckyProject.LocalizationManager.Services.Project.Requests
{
    public class UpdateProjectRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public string Locale { get; set; }
    }
}

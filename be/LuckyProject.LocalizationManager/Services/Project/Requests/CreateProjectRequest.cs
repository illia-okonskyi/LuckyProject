namespace LuckyProject.LocalizationManager.Services.Project.Requests
{
    public class CreateProjectRequest : UpdateProjectRequest
    {
        public string FilePath { get; init; }
    }
}

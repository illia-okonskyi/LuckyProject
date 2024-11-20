using System.Threading.Tasks;

namespace LuckyProject.AuthServer.Services.Init
{
    public interface IDbLayerInitService
    {
        Task InitDbAsync();
    }
}

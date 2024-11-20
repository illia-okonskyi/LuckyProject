using LuckyProject.Lib.Web.Models;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Web.Services
{
    public interface ILpHttpService : IHttpClientFactory
    {
        Task LpApiRequestAsync(
            HttpClient httpClient,
            LpApiHttpClientRequest request,
            CancellationToken cancellationToken = default);
        Task<TPayload> LpApiRequestAsync<TPayload>(
            HttpClient httpClient,
            LpApiHttpClientRequest request,
            CancellationToken cancellationToken = default);
    }
}

using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.Web.Constants;
using LuckyProject.Lib.Web.Exceptions;
using LuckyProject.Lib.Web.JsonConverters;
using LuckyProject.Lib.Web.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Web.Services
{
    public class LpHttpService : ILpHttpService
    {
        #region Internals & ctor
        private static readonly List<JsonConverter> DefaultJsonConverters = new()
        {
            new AbstractLpApiErrorJsonConverter()
        };

        private readonly IHttpClientFactory httpClientFactory;
        private readonly IJsonService jsonService;

        public LpHttpService(IHttpClientFactory httpClientFactory, IJsonService jsonService)
        {
            this.httpClientFactory = httpClientFactory;
            this.jsonService = jsonService;
        }
        #endregion

        #region IHttpClientFactory implementation
        public HttpClient CreateClient(string name) => httpClientFactory.CreateClient(name);
        #endregion

        #region LpApiRequestAsync
        public async Task LpApiRequestAsync(
            HttpClient httpClient,
            LpApiHttpClientRequest request,
            CancellationToken cancellationToken = default)
        {
            var jsonConverters = GetJsonConverters(request);
            var content = await LpApiRequestAsync(
                httpClient,
                request,
                jsonConverters,
                cancellationToken);
            var response = jsonService
                .DeserializeObject<LpApiResponse>(content, jsonConverters);
            if (!response.Success)
            {
                throw new LpApiRequestException(response.LpApiError);
            }
        }

        public async Task<TPayload> LpApiRequestAsync<TPayload>(
            HttpClient httpClient,
            LpApiHttpClientRequest request,
            CancellationToken cancellationToken = default)
        {
            var jsonConverters = GetJsonConverters(request);
            var content = await LpApiRequestAsync(
                httpClient,
                request,
                jsonConverters,
                cancellationToken);
            var response = jsonService
                .DeserializeObject<LpApiResponse<TPayload>>(content, jsonConverters);
            if (!response.Success)
            {
                throw new LpApiRequestException(response.LpApiError);
            }

            return response.LpApiPayload;
        }

        private async Task<string> LpApiRequestAsync(
            HttpClient httpClient,
            LpApiHttpClientRequest request,
            JsonConverter[] jsonConverters,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentException.ThrowIfNullOrEmpty(request.BaseUrl);

            var httpRequestMessage = new HttpRequestMessage(request.Method, GetFullUrl(request));
            if (string.IsNullOrEmpty(request.AccessToken))
            {
                httpRequestMessage.Headers.Add(
                    HeaderNames.Authorization,
                    $"{LpWebConstants.Auth.Jwt.TokenType} {request.AccessToken}");
            }

            foreach (var header in request.Headers)
            {
                httpRequestMessage.Headers.Add(header.Key, header.Value);
            }

            StringContent requestContent = null;
            if (request.Content != null)
            {
                requestContent = new StringContent(
                    jsonService.SerializeObject(request.Content, jsonConverters),
                    Encoding.UTF8,
                    MediaTypeNames.Application.Json);
            }

            if (requestContent != null)
            {
                httpRequestMessage.Content = requestContent;
            }

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
            var responseContent = await httpResponseMessage
                .Content
                .ReadAsStringAsync(cancellationToken);
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                throw new LpApiRequestException(
                    httpResponseMessage.StatusCode,
                    responseContent);
            }

            return responseContent;
        }

        private static string GetFullUrl(LpApiHttpClientRequest request)
        {
            if (request.Parameters.Count == 0)
            {
                return request.BaseUrl;
            }

            var queryBuilder = new QueryBuilder();
            foreach (var parameter in request.Parameters)
            {
                queryBuilder.Add(parameter.Key, parameter.Value);
            }
            var queryString = queryBuilder.ToQueryString().ToString();
            return request.BaseUrl + queryString;
        }

        private static JsonConverter[] GetJsonConverters(LpApiHttpClientRequest request)
        {
            var l = DefaultJsonConverters.ToList();
            l.AddRange(request.JsonConverters);
            return l.ToArray();
        }
        #endregion
    }
}

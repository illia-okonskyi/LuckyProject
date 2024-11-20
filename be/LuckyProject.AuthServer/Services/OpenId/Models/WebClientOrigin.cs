using System;

namespace LuckyProject.AuthServer.Services.OpenId.Models
{
    public record WebClientOrigin
    {
        public Uri BaseUrl { get; }
        public Uri RedirectUrl { get; }
        public Uri PostLogoutRedirectUrl { get; }

        public WebClientOrigin(Uri baseUrl, string redirect, string postLogoutRedirect = null)
        {
            if (baseUrl == null)
            {
                throw new ArgumentNullException(nameof(baseUrl));
            }

            if (string.IsNullOrEmpty(redirect))
            {
                throw new ArgumentNullException(nameof(redirect));
            }

            BaseUrl = baseUrl;
            RedirectUrl = new Uri(baseUrl, redirect);
            if (!string.IsNullOrEmpty(postLogoutRedirect))
            {
                PostLogoutRedirectUrl = new Uri(baseUrl, postLogoutRedirect);
            }
        }

        /// <summary>
        /// Internal service usage only
        /// </summary>
        public WebClientOrigin(Uri baseUrl, Uri redirectUrl, Uri postLogoutRedirectUrl)
        {
            BaseUrl = baseUrl;
            RedirectUrl = redirectUrl;
            PostLogoutRedirectUrl = postLogoutRedirectUrl;
        }
    }
}

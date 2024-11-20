using System;
using System.Security.Cryptography.X509Certificates;

namespace LuckyProject.AuthServer.Models
{
    public class WebServerOptions
    {
        public string PublicName { get; set; }
        public int Port { get; set; }
        public string Endpoint { get; set; }
        public Uri EndpointUri => new Uri(Endpoint);
        public X509Certificate2 Cert { get; set; }
    }
}

using System;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace IdentityServer.Common
{
    public class UrlBuilder : IUrlBuilder
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private UriBuilder _uriBuilder;

        public UrlBuilder(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IUrlBuilder Create(string domain = null)
        {
            if (string.IsNullOrWhiteSpace(domain))
            {
                var request = _httpContextAccessor.HttpContext.Request;
                domain = $"{request.Scheme}://{request.Host}";
            }

            _uriBuilder = new UriBuilder(domain);
            return this;
        }

        public IUrlBuilder Path(string path)
        {
            _uriBuilder.Path = path;
            return this;
        }

        public IUrlBuilder AddQuery(string key, string value)
        {
            var query = HttpUtility.ParseQueryString(_uriBuilder.Query);
            query.Add(key, value);
            _uriBuilder.Query = query.ToString();
            return this;
        }

        public new string ToString()
        {
            return _uriBuilder.ToString();
        }
    }
}
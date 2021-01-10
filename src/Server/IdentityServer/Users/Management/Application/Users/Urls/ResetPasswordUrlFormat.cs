using System;
using System.Web;

namespace IdentityServer.Users.Management.Application.Users.Urls
{
    public class ResetPasswordUrlFormat
    {
        public string UserId { get; set; }
        public string Token { get; set; }

        public string UrlFormat { get; set; }

        public new string ToString()
        {
            var uri = new UriBuilder(UrlFormat);
            var query = HttpUtility.ParseQueryString(uri.Query);
            query["userId"] = UserId;
            query["token"] = Token;
            uri.Query = query.ToString();
            return uri.ToString();
        }
    }
}

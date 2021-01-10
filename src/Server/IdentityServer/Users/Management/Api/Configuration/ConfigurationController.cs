using IdentityServer.Users.Management.Configs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace IdentityServer.Users.Management.Api.Configuration
{
    [AllowAnonymous]
    [Route("configurations")]
    public class ConfigurationController : Controller
    {
        private IdentityServerUserManagementConfig _options;

        public ConfigurationController(IOptions<IdentityServerUserManagementConfig> options)
        {
            _options = options.Value;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var request = HttpContext.Request;
            var host = $"{request.Scheme}://{request.Host}";

            var config = new ConfigurationResponse
            {
                CreateUserPath = $"{host}{_options.Paths.CreateUserPath}"
            };
            return Json(config);
        }

    }



}

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Management.Api
{
    [ApiController]
    [Route("identity/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : ControllerBase 
    {
        [HttpPost]
        public IActionResult Create()
        {
            return Ok();
        }
    }
}
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Management.Api
{
    [Route("identity/[controller]")]
    public class UsersController : AuthorizedController
    {
        [HttpPost]
        public IActionResult Create()
        {
            return Ok();
        }
    }
}
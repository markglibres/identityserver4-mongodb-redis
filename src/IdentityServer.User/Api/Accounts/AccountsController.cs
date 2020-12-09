using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Management.Api.Accounts
{
    public class AccountsController : AuthorizedController
    {
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            return Ok(new
            {
                isSuccess = true,
                username = request.Username
            });
        }

    }

    public class LoginRequest
    {
        public string Username { get; set; }
    }
}

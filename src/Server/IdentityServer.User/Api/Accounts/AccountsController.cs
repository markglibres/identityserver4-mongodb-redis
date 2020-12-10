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
                username = request.Username,
                password = request.Password,
                returnUrl = request.ReturnUrl
            });
        }

    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}

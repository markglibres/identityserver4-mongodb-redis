using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Management.Api
{
    [ApiController]
    [Authorize]
    public abstract class AuthorizedController : ControllerBase
    {

    }
}

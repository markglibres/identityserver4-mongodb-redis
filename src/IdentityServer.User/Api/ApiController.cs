using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Management.Api
{
    [Route("identity/[controller]")]
    [ApiController]
    [Authorize(Policy = Policies.UserManagement, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public abstract class ApiController : ControllerBase
    {

    }
}

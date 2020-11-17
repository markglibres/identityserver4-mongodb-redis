using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Management.Api
{
    [ApiController]
    [AllowAnonymous]
    public abstract class AnonymousController : ControllerBase
    {
        
    }
}
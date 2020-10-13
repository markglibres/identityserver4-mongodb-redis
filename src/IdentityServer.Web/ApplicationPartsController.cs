using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Web
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationPartsController : ControllerBase
    {
        public ApplicationPartsController()
        {
            
        }

        [HttpGet]
        [Route("test")]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
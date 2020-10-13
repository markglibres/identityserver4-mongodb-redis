using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers
{
    [Route("identity/[controller]")]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("test");
        }
    }
}
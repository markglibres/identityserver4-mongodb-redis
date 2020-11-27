using System.Threading.Tasks;
using IdentityServer.Management.Api.Users.RegisterUser;
using IdentityServer.Management.Application.Users.RegisterUser;
using IdentityServer.Management.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace IdentityServer.Management.Api.Users
{
    [Route("identity/[controller]")]
    public class UsersController : AuthorizedController
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public UsersController(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        [HttpPost]
        [AuthorizeForScopes(Scopes = new[] {"myapi.user "})]
        public async Task<IActionResult> Create([FromBody] RegisterUserRequest request)
        {
            var command = _mapper.Map<RegisterUserCommand>(request);
            var result = await _mediator.Send(command);
            var response = _mapper.Map<RegisterUserResponse>(result);

            return response.IsSuccess ? (IActionResult) Ok(response) : BadRequest(response);
        }
    }
}

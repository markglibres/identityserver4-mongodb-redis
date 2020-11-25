using System.Threading.Tasks;
using IdentityServer.Management.Api.Users.RegisterUser;
using IdentityServer.Management.Application.Users.RegisterUser;
using IdentityServer.Management.Common;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Create([FromBody] RegisterUserRequest request)
        {
            var command = _mapper.Map<RegisterUserCommand>(request);
            var result = await _mediator.Send(command);
            var response = _mapper.Map<RegisterUserResponse>(result);

            return response.IsSuccess ? (IActionResult) Ok(response) : BadRequest(response);
        }
    }
}

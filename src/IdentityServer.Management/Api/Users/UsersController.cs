using System.Threading.Tasks;
using IdentityServer.Management.Api.Requests;
using IdentityServer.Management.Application.Users.CreateUser;
using IdentityServer.Management.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Management.Api
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
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
        {
            var command = _mapper.Map<CreateUserCommand>(request);
            var result = await _mediator.Send(command);
            var response = _mapper.Map<CreateUserResponse>(result);
            
            return Ok(response);
        }
    }
}
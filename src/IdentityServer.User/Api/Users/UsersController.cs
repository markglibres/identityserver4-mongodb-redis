using System.Threading.Tasks;
using IdentityServer.Management.Api.Users.ConfirmEmail;
using IdentityServer.Management.Api.Users.ForgotPassword;
using IdentityServer.Management.Api.Users.RegisterUser;
using IdentityServer.Management.Application.Users.ConfirmEmail;
using IdentityServer.Management.Application.Users.ForgotPassword;
using IdentityServer.Management.Application.Users.RegisterUser;
using IdentityServer.Management.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

namespace IdentityServer.Management.Api.Users
{

    [Route("identity/[controller]")]
    public class UsersController : AuthorizedController
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public UsersController(
            IMapper mapper,
            IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;

        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RegisterUserRequest request)
        {
            var command = _mapper.Map<RegisterUserCommand>(request);
            command.ConfirmationUrl = new ConfirmationEmailUrlFormat
            {
                UrlFormat = $"{GetBaseUrl()}/{{{nameof(ConfirmationEmailUrlFormat.UserId)}}}/confirm/{{{nameof(ConfirmationEmailUrlFormat.Token)}}}"
            };

            var result = await _mediator.Send(command);
            var response = _mapper.Map<RegisterUserResponse>(result);

            return response.IsSuccess ? (IActionResult) Ok(response) : BadRequest(response);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("{userId}/confirm/{token}")]
        public async Task<IActionResult> Confirm([FromRoute] ConfirmEmailRequest request)
        {
            var query = _mapper.Map<ConfirmEmailQuery>(request);
            var result = await _mediator.Send(query);
            var response = _mapper.Map<ConfirmEmailResponse>(result);

            if (response.IsSuccess) return Ok();

            return NotFound(response.Errors);
        }

        [HttpPost]
        [Route("{Id}")]
        public async Task<IActionResult> ForgotPassword([FromRoute] ForgotPasswordRequest request)
        {
            var command = _mapper.Map<ForgotPasswordCommand>(request);
            command.ResetPasswordUrlFormat = $"{GetBaseUrl()}/resetpassword?token={{token}}";
            var result = await _mediator.Send(command);
            var response = _mapper.Map<ForgotPasswordResponse>(result);
            return Ok();
        }

        private string GetBaseUrl() => $"{Request.Scheme}://{Request.Host}{Request.Path}";
    }

}

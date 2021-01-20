using System.Threading.Tasks;
using IdentityServer.Common;
using IdentityServer.Users.Management.Api.Users.ConfirmEmail;
using IdentityServer.Users.Management.Api.Users.ForgotPassword;
using IdentityServer.Users.Management.Api.Users.RegisterUser;
using IdentityServer.Users.Management.Api.Users.ResetPassword;
using IdentityServer.Users.Management.Application.Users.ConfirmEmail;
using IdentityServer.Users.Management.Application.Users.ForgotPassword;
using IdentityServer.Users.Management.Application.Users.RegisterUser;
using IdentityServer.Users.Management.Application.Users.ResetPassword;
using IdentityServer.Users.Management.Application.Users.Urls;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Users.Management.Api.Users
{
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
            command.ConfirmUrlFormatter = (userId, token, returnUrl) => $"{GetBaseUrl()}/{userId}/confirm/{token}?ReturnUrl={returnUrl}";

            var result = await _mediator.Send(command);
            var response = _mapper.Map<RegisterUserResponse>(result);

            return response.IsSuccess ? (IActionResult) Ok(response) : BadRequest(response);
        }

        [HttpGet]
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
        [Route("forgot")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var command = _mapper.Map<ForgotPasswordCommand>(request);
            command.ResetPasswordUrl = new ResetPasswordUrlFormat
            {
                UrlFormat = request.ResetUrl
            };

            var result = await _mediator.Send(command);
            var response = _mapper.Map<ForgotPasswordResponse>(result);
            return Ok();
        }

        [HttpPost]
        [Route("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var command = _mapper.Map<ResetPasswordCommand>(request);
            var result = await _mediator.Send(command);
            var response = _mapper.Map<ResetPasswordResponse>(result);

            if (response.IsSuccess) return Ok();

            return NotFound(response.Errors);
        }

        private string GetBaseUrl()
        {
            return $"{Request.Scheme}://{Request.Host}{Request.Path}";
        }
    }
}
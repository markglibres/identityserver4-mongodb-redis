using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer.Management.Api.Users.ConfirmEmail;
using IdentityServer.Management.Api.Users.ForgotPassword;
using IdentityServer.Management.Api.Users.RegisterUser;
using IdentityServer.Management.Api.Users.ResetPassword;
using IdentityServer.Management.Application.Users.ConfirmEmail;
using IdentityServer.Management.Application.Users.ForgotPassword;
using IdentityServer.Management.Application.Users.RegisterUser;
using IdentityServer.Management.Application.Users.ResetPassword;
using IdentityServer.Management.Application.Users.Urls;
using IdentityServer.Management.Common;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

namespace IdentityServer.Management.Api.Users
{

    [Route("identity/[controller]")]
    [Authorize(Policy = Policies.UserManagement, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : ApiController
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
            command.ConfirmUrl = new ConfirmEmailUrlFormat
            {
                UrlFormat = $"{GetBaseUrl()}/{{{nameof(ConfirmEmailUrlFormat.UserId)}}}/confirm/{{{nameof(ConfirmEmailUrlFormat.Token)}}}"
            };

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

        private string GetBaseUrl() => $"{Request.Scheme}://{Request.Host}{Request.Path}";
    }
}

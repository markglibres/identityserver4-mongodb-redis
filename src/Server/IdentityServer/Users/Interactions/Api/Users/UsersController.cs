using System.Threading.Tasks;
using IdentityServer.Common;
using IdentityServer.Users.Interactions.Api.Users.ConfirmEmail;
using IdentityServer.Users.Interactions.Api.Users.ForgotPassword;
using IdentityServer.Users.Interactions.Api.Users.RegisterUser;
using IdentityServer.Users.Interactions.Api.Users.ResetPassword;
using IdentityServer.Users.Interactions.Application.Users.ConfirmEmail;
using IdentityServer.Users.Interactions.Application.Users.ForgotPassword;
using IdentityServer.Users.Interactions.Application.Users.RegisterUser;
using IdentityServer.Users.Interactions.Application.Users.ResetPassword;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Users.Interactions.Api.Users
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

            var result = await _mediator.Send(command);
            var response = _mapper.Map<RegisterUserResponse>(result);

            return response.IsSuccess ? (IActionResult) Ok(response) : BadRequest(response);
        }

        [HttpGet]
        [Route("confirm")]
        public async Task<IActionResult> Confirm(string userId, string token, string returnUrl = "")
        {
            var query = new ConfirmEmailQuery
            {
                UserId = userId,
                Token = token,
                ReturnUrl = returnUrl
            };
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

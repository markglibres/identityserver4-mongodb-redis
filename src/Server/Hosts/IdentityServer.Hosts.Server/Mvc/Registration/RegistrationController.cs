using System;
using System.Threading.Tasks;
using System.Web;
using HandlebarsDotNet.Helpers;
using IdentityServer.Common;
using IdentityServer.Hosts.Mvc.ViewModels;
using IdentityServer.Users.Management.Api.Users.ConfirmEmail;
using IdentityServer.Users.Management.Api.Users.ForgotPassword;
using IdentityServer.Users.Management.Api.Users.ResetPassword;
using IdentityServer.Users.Management.Application.Users;
using IdentityServer.Users.Management.Application.Users.ConfirmEmail;
using IdentityServer.Users.Management.Application.Users.ForgotPassword;
using IdentityServer.Users.Management.Application.Users.RegisterUser;
using IdentityServer.Users.Management.Application.Users.ResetPassword;
using IdentityServer.Users.Management.Configs;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ForgotPasswordRequest = IdentityServer.Hosts.Mvc.ViewModels.ForgotPasswordRequest;
using ResetPasswordRequest = IdentityServer.Hosts.Mvc.ViewModels.ResetPasswordRequest;

namespace IdentityServer.Hosts.Mvc.Controllers
{
    [AllowAnonymous]
    public class RegistrationController : Controller
    {
        private readonly IClientSecretValidator _clientSecretValidator;
        private readonly IIdentityServerInteractionService _interactionService;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IdentityServerUserManagementConfig _options;
        private readonly ITokenValidator _tokenValidator;

        public RegistrationController(IIdentityServerInteractionService interactionService,
            IClientSecretValidator clientSecretValidator,
            IOptions<IdentityServerUserManagementConfig> options,
            ITokenValidator tokenValidator,
            IMapper mapper,
            IMediator mediator)
        {
            _interactionService = interactionService;
            _clientSecretValidator = clientSecretValidator;
            _options = options.Value;
            _tokenValidator = tokenValidator;
            _mapper = mapper;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Register(string returnUrl)
        {
            return View("CreateUser",new CreateUserRequest
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpGet]
        public async Task<IActionResult> CreateUser(string token, string returnUrl)
        {
            var validationResult = await _tokenValidator.ValidateAccessTokenAsync(token, _options.Scope);
            if (validationResult.IsError) throw new Exception(validationResult.Error);

            return View(new CreateUserRequest
            {
                Token = token,
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserRequest request, string button)
        {
            if (!ModelState.IsValid) return View(request);

            if (!string.IsNullOrWhiteSpace(request.Token))
            {
                var validationResult = await _tokenValidator.ValidateAccessTokenAsync(request.Token, _options.Scope);
                if (validationResult.IsError) throw new Exception(validationResult.Error);
            }

            var command = _mapper.Map<RegisterUserCommand>(request);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(nameof(request.Validations), string.Join(" ", result.Errors));
                return View(request);
            }

            var model = _mapper.Map<UserCreatedModel>(result, createdModel => createdModel.Email = request.Email);
            return View("UserCreated", model);
        }

        [HttpGet]
        public async Task<IActionResult> Confirm(string userId, string token, string returnUrl = "")
        {
            var query = new ConfirmEmailQuery { Token = token, UserId = userId, ReturnUrl = returnUrl };
            var result = await _mediator.Send(query);
            var response = _mapper.Map<ConfirmEmailResponse>(result);

            if (response.IsSuccess)
            {
                return View( "UpdatePassword", new UpdatePasswordModel
                {
                    Token = token,
                    ReturnUrl = response.ReturnUrl,
                    UserId = userId,
                    ResetPasswordToken = response.ResetPasswordToken
                });
            }

            return View("ConfirmError", new ErrorModel {Message = string.Join(" ", response.Errors)});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordModel request, string button)
        {
            if (!ModelState.IsValid) return View(request);

            var command = _mapper.Map<UpdatePasswordCommand>(
                request,
                passwordCommand => passwordCommand.NewPassword = request.Password);

            var result = await _mediator.Send(command);

            if (result.IsSuccess) return Redirect(request.ReturnUrl);

            ModelState.AddModelError(nameof(request.Password), string.Join(" ", result.Errors));
            return View(request);

        }

        [HttpGet]
        public async Task<IActionResult> ForgotPassword(string returnUrl)
        {
            return View("ForgotPassword",new ForgotPasswordRequest
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request, string button)
        {
            var command = _mapper.Map<ForgotPasswordCommand>(request);
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return View("ResetPasswordSent", new ResetPasswordSent
                {
                    Email = request.Email
                });

            ModelState.AddModelError(nameof(request.Email), result.Message);
            return View(request);

        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string userId, string token, string returnUrl)
        {
            return View("ResetPassword",new ResetPasswordRequest
            {
                UserId = userId,
                Token = token,
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            var command = _mapper.Map<ResetPasswordCommand>(request);
            var result = await _mediator.Send(command);
            var response = _mapper.Map<ResetPasswordResponse>(result);

            if (response.IsSuccess) return Redirect(request.ReturnUrl);

            ModelState.AddModelError(nameof(request.ValidationMessage), string.Join(" ", response.Errors));
            return View(request);
        }

        private string GetCurrentPath()
        {
            return $"{GetBaseUrl()}/{Request.RouteValues["controller"]}";
        }

        private string GetBaseUrl()
        {
            return $"{Request.Scheme}://{Request.Host}";
        }
    }


}

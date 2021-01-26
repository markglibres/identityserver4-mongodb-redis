using System;
using System.Threading.Tasks;
using IdentityServer.Common;
using IdentityServer.Hosts.Mvc.ViewModels;
using IdentityServer.HostServer.Mvc.ViewModels;
using IdentityServer.Users.Management.Api.Users.ConfirmEmail;
using IdentityServer.Users.Management.Api.Users.ResetPassword;
using IdentityServer.Users.Management.Api.Users.UpdateProfile;
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

namespace IdentityServer.HostServer.Mvc.Registration
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
            var context = await _interactionService.GetAuthorizationContextAsync(returnUrl);
            if(context == null) throw new Exception("Invalid context");

            return View("CreateUser", new CreateUserModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpGet]
        public async Task<IActionResult> CreateUser(string token, string returnUrl)
        {
            var validationResult = await _tokenValidator.ValidateAccessTokenAsync(token, _options.Scope);
            if (validationResult.IsError) throw new Exception(validationResult.Error);

            return View(new CreateUserModel
            {
                Token = token,
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserModel request, string button)
        {
            if (!ModelState.IsValid) return View(request);

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
        public async Task<IActionResult> Confirm(string userId, string token, string returnUrl)
        {
            var query = new ConfirmEmailQuery {Token = token, UserId = userId, ReturnUrl = returnUrl};
            var result = await _mediator.Send(query);
            var response = _mapper.Map<ConfirmEmailResponse>(result);

            if (response.IsSuccess)
                return View("UpdatePassword", new UpdatePasswordModel
                {
                    Token = token,
                    ReturnUrl = response.ReturnUrl,
                    UserId = userId,
                    ResetPasswordToken = response.ResetPasswordToken
                });

            return View("ConfirmError", new ErrorModel {Message = string.Join(" ", response.Errors)});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordModel request, string button)
        {
            if (!ModelState.IsValid) return View(request);

            var updatePasswordCommand = _mapper.Map<UpdatePasswordCommand>(
                request,
                passwordCommand => passwordCommand.NewPassword = request.Password);
            var updatePasswordCommandResult = await _mediator.Send(updatePasswordCommand);

            var updateProfileCommand = _mapper.Map<UpdateProfileCommand>(request);
            await _mediator.Send(updateProfileCommand);

            if (updatePasswordCommandResult.IsSuccess) return Redirect(request.ReturnUrl);

            ModelState.AddModelError(nameof(request.Password), string.Join(" ", updatePasswordCommandResult.Errors));
            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> ForgotPassword(string returnUrl)
        {
            return View("ForgotPassword", new ForgotPasswordModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel request, string button)
        {
            var command = _mapper.Map<ForgotPasswordCommand>(request);
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return View("ResetPasswordSent", new ResetPasswordSentModel
                {
                    Email = request.Email
                });

            ModelState.AddModelError(nameof(request.Email), result.Message);
            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string userId, string token, string returnUrl)
        {
            return View("ResetPassword", new ResetPasswordModel
            {
                UserId = userId,
                Token = token,
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel request)
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

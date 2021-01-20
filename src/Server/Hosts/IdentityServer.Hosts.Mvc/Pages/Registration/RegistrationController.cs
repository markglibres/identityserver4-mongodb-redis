using System;
using System.Threading.Tasks;
using System.Web;
using HandlebarsDotNet.Helpers;
using IdentityServer.Common;
using IdentityServer.Hosts.Mvc.ViewModels;
using IdentityServer.Users.Management.Api.Users.ConfirmEmail;
using IdentityServer.Users.Management.Application.Users.ConfirmEmail;
using IdentityServer.Users.Management.Application.Users.RegisterUser;
using IdentityServer.Users.Management.Configs;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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
        public async Task<IActionResult> CreateUser(string token)
        {
            var validationResult = await _tokenValidator.ValidateAccessTokenAsync(token, _options.Scope);
            if (validationResult.IsError) throw new Exception(validationResult.Error);

            return View(new CreateUserRequest
            {
                Token = token
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

            var command = _mapper.Map<RegisterUserCommand>(request,
                userCommand => userCommand.PlainTextPassword = request.Password);
            command.ConfirmUrlFormatter = (userId, token, returnUrl) =>
                $"{GetCurrentPath()}/confirm?userId={userId}&token={token}&returnUrl={HttpUtility.UrlEncode(returnUrl)}";

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
                return View( "UpdateProfile", new UpdateProfileModel
                {
                    Token = token,
                    ReturnUrl = response.ReturnUrl
                });
            }

            return View("ConfirmError", new ErrorModel {Message = string.Join(" ", response.Errors)});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UpdateProfileModel request, string button)
        {
            //return View("ProfileUpdated", new ProfileUpdatedModel());
            return Redirect(request.ReturnUrl);
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

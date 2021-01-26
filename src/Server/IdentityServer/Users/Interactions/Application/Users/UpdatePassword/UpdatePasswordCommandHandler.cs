using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Common;
using IdentityServer.Users.Authorization.Services;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServer.Users.Interactions.Application.Users.UpdatePassword
{
    public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, UpdatePasswordCommandResult>
    {
        private readonly ITokenService _tokenService;
        private readonly ITokenValidator _tokenValidator;
        private readonly UserManager<ApplicationUser> _userManager;

        public UpdatePasswordCommandHandler(
            UserManager<ApplicationUser> userManager,
            ITokenValidator tokenValidator,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenValidator = tokenValidator;
            _tokenService = tokenService;
        }

        public async Task<UpdatePasswordCommandResult> Handle(UpdatePasswordCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) throw new DomainException($"User {request.UserId} not found");

            var token = Base64UrlEncoder.Decode(request.ResetPasswordToken);
            var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

            return new UpdatePasswordCommandResult
            {
                IsSuccess = result.Succeeded,
                Errors = result.Errors.Select(e => e.Description)
            };
        }
    }
}
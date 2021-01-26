using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Common;
using IdentityServer.Users.Authorization.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServer.Users.Interactions.Application.Users.ConfirmEmail
{
    public class ConfirmEmailQueryHandler : IRequestHandler<ConfirmEmailQuery, ConfirmEmailQueryResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ConfirmEmailQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ConfirmEmailQueryResult> Handle(ConfirmEmailQuery request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) throw new DomainException($"User {request.UserId} is not found");

            var decodedToken = Base64UrlEncoder.Decode(request.Token);
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Base64UrlEncoder.Encode(token);

            return new ConfirmEmailQueryResult
            {
                IsSuccess = result.Succeeded,
                Errors = result.Errors.Select(e => e.Description),
                ReturnUrl = request.ReturnUrl,
                ResetPasswordToken = encodedToken
            };
        }
    }
}
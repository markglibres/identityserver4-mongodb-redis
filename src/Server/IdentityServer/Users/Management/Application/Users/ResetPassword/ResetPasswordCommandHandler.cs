using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Common;
using IdentityServer.Users.Authorization.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServer.Users.Management.Application.Users.ResetPassword
{
    public class ResetPasswordCommandHandler: IRequestHandler<ResetPasswordCommand, ResetPasswordCommandResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ResetPasswordCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ResetPasswordCommandResult> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if(user == null) throw  new DomainException($"User {request.UserId} not found");

            var decodedToken = Base64UrlEncoder.Decode(request.Token);

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.Password);

            return new ResetPasswordCommandResult
            {
                IsSuccess = result.Succeeded,
                Errors = result.Errors.Select(e => e.Description)
            };
        }
    }
}

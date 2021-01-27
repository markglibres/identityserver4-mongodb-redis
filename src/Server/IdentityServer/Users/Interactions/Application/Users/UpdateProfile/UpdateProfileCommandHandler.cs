using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Common;
using IdentityServer.Users.Authorization.Abstractions;
using IdentityServer.Users.Authorization.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Users.Interactions.Application.Users.UpdateProfile
{
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Unit>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService<ApplicationUser> _userService;

        public UpdateProfileCommandHandler(
            UserManager<ApplicationUser> userManager,
            IUserService<ApplicationUser> userService)
        {
            _userManager = userManager;
            _userService = userService;
        }

        public async Task<Unit> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) throw new DomainException($"User {request.UserId} not found");

            user.Firstname = request.Firstname;
            user.Lastname = request.Lastname;

            await _userService.Update(user, cancellationToken);
            return Unit.Value;
        }
    }
}
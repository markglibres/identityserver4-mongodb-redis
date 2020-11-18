using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Users;
using IdentityServer.Users.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Management.Application.Users.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserCommandResult>
    {
        private readonly ILogger<CreateUserCommandHandler> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService<ApplicationUser> _userService;
        
        public CreateUserCommandHandler(
            ILogger<CreateUserCommandHandler> logger,
            UserManager<ApplicationUser> userManager,
            IUserService<ApplicationUser> userService)
        {
            _logger = logger;
            _userManager = userManager;
            _userService = userService;
        }
        public async Task<CreateUserCommandResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userService.GetByUsername(request.Email, cancellationToken);
            if(user != null)
            {
                _logger.LogError($"User {request.Email} already exists");
                return new CreateUserCommandResult
                {
                    IsSuccess = false
                };
            }
            
            user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = request.Email,
                UserName = request.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, request.PlainTextPassword);
            var response = new CreateUserCommandResult
            {
                Id = result.Succeeded ? user.Id : string.Empty,
                IsSuccess = result.Succeeded,
                Errors = result.Errors.Select(e => e.Description)
            };
            
            return response;
        }
    }
}
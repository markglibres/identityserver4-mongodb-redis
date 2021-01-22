using MediatR;

namespace IdentityServer.Users.Management.Application.Users.UpdateProfile
{
    public class UpdateProfileCommand : IRequest<UpdateProfileCommandResult>
    {
        public string UserId { get; set; }
    }

    public class UpdateProfileCommandResult
    {
    }
}

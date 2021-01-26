using MediatR;

namespace IdentityServer.Users.Management.Api.Users.UpdateProfile
{
    public class UpdateProfileCommand : IRequest<Unit>
    {
        public string UserId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }
}

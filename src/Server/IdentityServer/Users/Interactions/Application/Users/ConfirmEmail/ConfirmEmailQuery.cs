using MediatR;

namespace IdentityServer.Users.Interactions.Application.Users.ConfirmEmail
{
    public class ConfirmEmailQuery : IRequest<ConfirmEmailQueryResult>
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public string ReturnUrl { get; set; }
    }
}
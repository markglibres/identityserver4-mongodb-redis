using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Users.Management.Application.Users.ConfirmEmail
{
    public class ConfirmEmailQuery : IRequest<ConfirmEmailQueryResult>
    {
        [FromRoute]
        public string UserId { get; set; }
        [FromQuery]
        public string Token { get; set; }
    }


}

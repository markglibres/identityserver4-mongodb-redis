using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Users.Authorization.Services
{
    public class ApplicationUser : IdentityUser
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }
}
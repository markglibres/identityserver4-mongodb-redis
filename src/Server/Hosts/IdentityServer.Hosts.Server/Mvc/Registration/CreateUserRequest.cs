using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Hosts.Mvc.ViewModels
{
    public class CreateUserRequest
    {
        [Required] public string Email { get; set; }

        public string Validations { get; set; }

        public string Token { get; set; }
        public string ReturnUrl { get; set; }
        public string RedirectUrl { get; set; }
    }
}

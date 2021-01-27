using System.ComponentModel.DataAnnotations;

namespace IdentityServer.HostServer.Mvc.ViewModels
{
    public class CreateUserModel
    {
        [Required] public string Email { get; set; }

        public string Validations { get; set; }

        public string Token { get; set; }
        public string ReturnUrl { get; set; }
    }
}
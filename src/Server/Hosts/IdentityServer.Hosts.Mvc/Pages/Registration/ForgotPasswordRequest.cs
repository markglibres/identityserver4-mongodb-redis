using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Hosts.Mvc.ViewModels
{
    public class ForgotPasswordRequest
    {
        [Required] public string Email { get; set; }
        public string ReturnUrl { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace IdentityServer.HostServer.Mvc.ViewModels
{
    public class ForgotPasswordModel
    {
        [Required] public string Email { get; set; }
        public string ReturnUrl { get; set; }
    }
}
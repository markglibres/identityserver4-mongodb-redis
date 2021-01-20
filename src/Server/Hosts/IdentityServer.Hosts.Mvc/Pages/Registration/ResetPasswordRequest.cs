using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Hosts.Mvc.ViewModels
{
    public class ResetPasswordRequest
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public string ReturnUrl { get; set; }
        [Required] public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string ConfirmPassword { get; set; }
        public string ValidationMessage { get; set; }
    }
}

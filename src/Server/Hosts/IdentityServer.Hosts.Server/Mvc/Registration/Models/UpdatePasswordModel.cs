using System.ComponentModel.DataAnnotations;

namespace IdentityServer.HostServer.Mvc.ViewModels
{
    public class UpdatePasswordModel
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        [Required]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string ConfirmPassword { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string ReturnUrl { get; set; }
        [Required]
        public string ResetPasswordToken { get; set; }
    }
}

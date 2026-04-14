using System.ComponentModel.DataAnnotations;

namespace MVC.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please Enter an Email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please Enter a Password")]
        public string Password { get; set; }

        public bool RememberMe { get; set; } = false;
    }
}

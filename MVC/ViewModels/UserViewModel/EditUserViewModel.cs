using System.ComponentModel.DataAnnotations;

namespace MVC.ViewModels.UserViewModel
{
    public class EditUserViewModel
    {

        [Required(ErrorMessage = "UserId is missing")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Please Enter a Name")]
        [StringLength(30)]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Name can only contain letters and numbers")]
        public string Name { get; set; }


        [Required(ErrorMessage = "Please Enter an Email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(100)]
        public string Email { get; set; }


        [Required(ErrorMessage = "Please enter an phone number")]
        [Phone(ErrorMessage = "Please eneter a valid phone number")]
        public string Phonenumber { get; set; }


        [Required(ErrorMessage = "Please Enter a Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        public List<string> roles { get; set; } = new();

        [Required(ErrorMessage = "Var god välj en roll")]
        public string selectedRole { get; set; }
    }
}

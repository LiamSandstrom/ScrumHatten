using System.ComponentModel.DataAnnotations;

namespace MVC.Models.Account
{
    public class RegisterViewModel
    {

        [Required(ErrorMessage = "Please Enter a Name")]
        [StringLength(30)]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Name can only contain letters and numbers")]
        public required string Name { get; set; }


        [Required(ErrorMessage = "Please Enter an Email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(100)]
        public required string Email { get; set; }




        [Required(ErrorMessage ="Please enter an phonenumer")]
        [Phone(ErrorMessage ="Please eneter a valid pohonenumer")]
        public required string Phonenumber { get; set; }




        [Required(ErrorMessage = "Please Enter a Password")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Please confirm Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password")]
        public required string ConfirmPassword { get; set; }


    }
}

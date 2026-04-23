using System.ComponentModel.DataAnnotations;

namespace MVC.ViewModels.CustomerViewModels
{
    public class CreateCustomerViewModel
    {
        [Required(ErrorMessage = "Vänligen ange ett namn")]
        [StringLength(30)]
        [RegularExpression(@"^[\p{L}0-9 ]+$", ErrorMessage = "Namnet får endast innehålla bokstäver, siffror och mellanslag")]

        public string Name { get; set; }


        [Required(ErrorMessage = "Vänligen ange ett telefonnummer")]
        [Phone(ErrorMessage = "Vänligen ange ett giligt telefonnummer")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vänligen ange ett email")]
        [EmailAddress(ErrorMessage = "Vänligen ange en giligt email")]
        [StringLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vänligen ange ett adress")]
        [StringLength(100)]
        public string Adress { get; set; }

        [Required(ErrorMessage = "Vänligen ange ett postnummer")]
        [StringLength(100)]
        public string ZipCode { get; set; }


        [Required(ErrorMessage = "Vänligen ange en stad")]
        [StringLength(100)]
        public string City { get; set; }

        [Required(ErrorMessage = "Vänligen ange ett land")]
        [StringLength(100)]
        public string Country { get; set; }

        public double Discount { get; set; }



    }


}

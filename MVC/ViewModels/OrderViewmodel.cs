using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MVC.ViewModels
{
    public class OrderRowViewModel
    {
        public string? HatId { get; set; } // ingen [Required] — custom har null

        [Range(1, int.MaxValue, ErrorMessage = "Antal måste vara minst 1")]
        public int Quantity { get; set; } = 1;

        public decimal? CustomPrice { get; set; }
        public string? CustomDescription { get; set; }
        public string? Size { get; set; }
        public IFormFile? CustomImage { get; set; }
        public List<MaterialRowViewModel> Materials { get; set; } = new();
    }

    public class MaterialRowViewModel
    {
        public string Id { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderViewModel
    {
        [MinLength(1, ErrorMessage = "Minst en hatt krävs")]
        public List<OrderRowViewModel> Rows { get; set; } = new();

        [ValidateNever]
        public List<Order> OrderList { get; set; }
        public List<SelectListItem> Hats { get; set; } = new();

        [Range(0, double.MaxValue, ErrorMessage = "Ogiltig tid")]
        public decimal TimeToMake { get; set; }

        [Required(ErrorMessage = "Datum krävs")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(OrderViewModel), nameof(ValidateDate))]
        public DateTime DateToFinish { get; set; } = DateTime.Today;

        [Range(0, double.MaxValue, ErrorMessage = "Ogiltig transportkostnad")]
        public decimal TransportPrice { get; set; }

        public decimal Moms { get; set; }
        public bool FastOrder { get; set; } = false;

        [Required(ErrorMessage = "Välj utförare")]
        public string SelectedUserId { get; set; }

        public List<SelectListItem> Users { get; set; }

        [Required(ErrorMessage = "Välj kund")]
        public string SelectedCustomerId { get; set; }

        public List<SelectListItem> Customers { get; set; } = new();

        public static ValidationResult ValidateDate(DateTime date, ValidationContext ctx)
        {
            if (date.Date < DateTime.Today)
                return new ValidationResult("Datum kan inte vara i det förflutna");
            return ValidationResult.Success!;
        }
    }
}

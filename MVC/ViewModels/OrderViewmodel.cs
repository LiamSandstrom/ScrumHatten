using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MVC.ViewModels
{
    public class OrderRowViewModel
    {
        public string? HatId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Antal måste vara minst 1")]
        public int Quantity { get; set; } = 1;

        public decimal? CustomPrice { get; set; }
        public string? CustomDescription { get; set; }
        public string? Size { get; set; }
        public IFormFile? CustomImage { get; set; }
        public List<HatMaterial> Materials { get; set; } = new();
    }

    public class OrderViewModel
    {
        [MinLength(1, ErrorMessage = "Minst en hatt krävs")]
        public List<OrderRowViewModel> Rows { get; set; } = new();

        [ValidateNever]
        public List<Order> OrderList { get; set; }

        [ValidateNever]
        public List<SelectListItem> Users { get; set; }

        [Range(0, 100)]
        public decimal Discount { get; set; } = 0;

        [Range(0, 100)]
        public decimal Customs { get; set; } = 0;

        [ValidateNever]
        public List<SelectListItem> Customers { get; set; } = new();

        [Range(0, double.MaxValue)]
        public decimal TimeToMake { get; set; }

        [Required, DataType(DataType.Date)]
        [CustomValidation(typeof(OrderViewModel), nameof(ValidateDate))]
        public DateTime DateToFinish { get; set; } = DateTime.Today;

        [Range(0, double.MaxValue)]
        public decimal TransportPrice { get; set; }

        public bool FastOrder { get; set; } = false;

        [Required(ErrorMessage = "Välj utförare")]
        public string SelectedUserId { get; set; }

        [Required(ErrorMessage = "Välj kund")]
        public string SelectedCustomerId { get; set; }

        public static ValidationResult ValidateDate(DateTime date, ValidationContext ctx)
        {
            if (date.Date < DateTime.Today)
                return new ValidationResult("Datum kan inte vara i det förflutna");
            return ValidationResult.Success!;
        }
    }
}

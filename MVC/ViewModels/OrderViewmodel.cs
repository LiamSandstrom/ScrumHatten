using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;

namespace MVC.ViewModels
{
    public class OrderRowViewModel
    {
        public required string Type { get; set; }
        public int? HatId { get; set; }
        public int Quantity { get; set; }
    }
    public class OrderViewModel
    {
        [Required, StringLength(100)]
        public string Name { get; set; }

        public List<OrderRowViewModel> Rows { get; set; } = new();

        public List<SelectListItem> StockHats { get; set; }
        public List<SelectListItem> CustomHats { get; set; }

        public decimal TimeToMake { get; set; }

        public DateTime DateToFinish { get; set; } = DateTime.Today;

        public int HatAmount { get; set; }

        public decimal TransportPrice { get; set; }

        public decimal Moms { get; set; }

        public Boolean FastOrder { get; set; }

        public string SelectedUserId { get; set; }
        public List<SelectListItem> Users { get; set; }

    }


}

using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace MVC.ViewModels
{
    public class AddHatViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public IFormFile ImageFile { get; set; }

        public List<HatMaterialInputViewModel> Materials { get; set; } = new();
    }
}
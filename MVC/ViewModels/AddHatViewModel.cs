using Microsoft.AspNetCore.Http;

namespace MVC.ViewModels
{
    public class AddHatViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }

        // Bildfil från datorn
        public IFormFile ImageFile { get; set; }
    }
}
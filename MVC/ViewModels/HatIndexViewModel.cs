using Models;
using System.Collections.Generic;

namespace MVC.ViewModels
{
    public class HatIndexViewModel
    {
        public List<Hat> Hats { get; set; } = new();
        public List<Material> Materials { get; set; } = new();
    }
}
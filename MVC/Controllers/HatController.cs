using Microsoft.AspNetCore.Mvc;
using BL.Interfaces;
using Models;

namespace MVC.Controllers
{
    public class HatController : Controller
    {
        private readonly IHatService _hatService;

        public HatController(IHatService hatService)
        {
            _hatService = hatService;
        }

        // // HAR SVÅRT ATT KOPPLA MOT DATABASEN
        public IActionResult Index()
        {
            List<Hat> hats = _hatService.GetAllHats() ?? new List<Hat>();
            return View(hats);
        }

        //TESATDATA
        //public IActionResult Index()
        //{
        //    List<Hat> hats = new List<Hat>
        //    {
        //        new Hat
        //        {
        //            Id = "1",
        //            Name = "Sommarhatt",
        //            Description = "En ljus och fin sommarhatt.",
        //            ImageUrl = "https://via.placeholder.com/300x250",
        //            Price = 299,
        //            Quantity = 5
        //        },
        //        new Hat
        //        {
        //            Id = "2",
        //            Name = "Vinterhatt",
        //            Description = "En varm hatt för kalla dagar.",
        //            ImageUrl = "https://via.placeholder.com/300x250",
        //            Price = 399,
        //            Quantity = 2
        //        }
        //    };

        //    return View(hats);

    }
    }


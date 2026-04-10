using BL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    public class HatController : Controller
    {
        private readonly IHatService _hatService;

        public HatController(IHatService hatService)
        {
            _hatService = hatService;
        }

        public IActionResult Index()
        {
            var hats = _hatService.GetAllHats();
            return View(hats);
        }

        public IActionResult Details(string id)
        {
            var hat = _hatService.GetHatById(id);

            if (hat == null)
                return NotFound();

            return View(hat);
        }
    }
}

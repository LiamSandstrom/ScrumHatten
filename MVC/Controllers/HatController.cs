using Microsoft.AspNetCore.Mvc;
using BL.Interfaces;
using Models;
using MVC.ViewModels;

namespace MVC.Controllers
{
    public class HatController : Controller
    {
        private readonly IHatService _hatService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HatController(IHatService hatService, IWebHostEnvironment webHostEnvironment)
        {
            _hatService = hatService;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Hat> hats = _hatService.GetAllHats() ?? new List<Hat>();
            return View(hats);
        }

        [HttpPost]
        public IActionResult AddHat(AddHatViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                TempData["ErrorMessage"] = "Du måste ange ett namn på hatten.";
                return RedirectToAction("Index");
            }

            string imagePath = "";

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "hats");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                string fullPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    model.ImageFile.CopyTo(stream);
                }

                imagePath = "/images/hats/" + fileName;
            }

            Hat newHat = new Hat
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Quantity = model.Quantity,
                ImageUrl = imagePath
            };

            try
            {
                _hatService.AddHat(newHat);
                TempData["SuccessMessage"] = "Hatten har lagts till!";
            }
            catch
            {
                TempData["ErrorMessage"] = "Kunde inte spara hatten i databasen.";
            }

            return RedirectToAction("Index");
        }
    }
}

//using Microsoft.AspNetCore.Mvc;
//using BL.Interfaces;
//using Models;

//namespace MVC.Controllers
//{
//    public class HatController : Controller
//    {
//        private readonly IHatService _hatService;

//        public HatController(IHatService hatService)
//        {
//            _hatService = hatService;
//        }

//        // // HAR SVÅRT ATT KOPPLA MOT DATABASEN
//        public IActionResult Index()
//        {
//            List<Hat> hats = _hatService.GetAllHats() ?? new List<Hat>();
//            return View(hats);
//        }

//        [HttpPost]
//        public IActionResult AddHat(Hat hat)
//        {
//            if (string.IsNullOrWhiteSpace(hat.Name))
//            {
//                TempData["ErrorMessage"] = "Du måste ange ett namn på hatten.";
//                return RedirectToAction("Index");
//            }

//            _hatService.AddHat(hat);
//            TempData["SuccessMessage"] = "Hatten har lagts till!";
//            return RedirectToAction("Index");
//        }

//    }
//}


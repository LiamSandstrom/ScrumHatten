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

        [HttpPost]
        public IActionResult DeleteHat(string id)
        {
            try
            {
                _hatService.DeleteHat(id);
                TempData["SuccessMessage"] = "Hatten har tagits bort.";
            }
            catch
            {
                TempData["ErrorMessage"] = "Kunde inte ta bort hatten.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateHat(Hat hat, IFormFile? ImageFile)
        {
            try
            {
                // Om en ny bild valts
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "hats");

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    string fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);

                    string fullPath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        ImageFile.CopyTo(stream);
                    }

                    hat.ImageUrl = "/images/hats/" + fileName;
                }

                _hatService.UpdateHat(hat);

                TempData["SuccessMessage"] = "Hatten uppdaterades!";
            }
            catch
            {
                TempData["ErrorMessage"] = "Kunde inte uppdatera hatten.";
            }

            return RedirectToAction("Index");
        }
    }
}
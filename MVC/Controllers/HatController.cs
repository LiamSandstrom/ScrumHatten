using Microsoft.AspNetCore.Mvc;
using BL.Interfaces;
using Models;
using MVC.ViewModels;
using Repository;
using System.Linq;

namespace MVC.Controllers
{
    public class HatController : Controller
    {
        private readonly IHatService _hatService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMaterialRepository _materialRepository;

        public HatController(IHatService hatService, IWebHostEnvironment webHostEnvironment, IMaterialRepository materialRepository)
        {
            _hatService = hatService;
            _webHostEnvironment = webHostEnvironment;
            _materialRepository = materialRepository;
        }

        //HäMTA ALLA HATTAR OCH MATERIALER

        public async Task<IActionResult> Index()
        {
            var hats = _hatService.GetAllHats() ?? new List<Hat>();
            var materials = await _materialRepository.GetAllMaterialsAsync();

            var model = new HatIndexViewModel
            {
                Hats = hats,
                Materials = materials
            };

            return View(model);


            ///LÄGG TILL HATT
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

            var hatMaterials = model.Materials?
                .Where(m => !string.IsNullOrWhiteSpace(m.MaterialId) && m.Amount > 0)
                .Select(m => new HatMaterial
                {
                    MaterialId = m.MaterialId,
                    Amount = m.Amount
                })
                .ToList() ?? new List<HatMaterial>();

            Hat newHat = new Hat
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Quantity = model.Quantity,
                ImageUrl = imagePath,
                Materials = hatMaterials
            };

            try
            {
                _hatService.AddHat(newHat);
                TempData["SuccessMessage"] = "Hatten har lagts till!";
            }
            catch
            {
                TempData["ErrorMessage"] = "Kunde inte spara hatten.";
            }

            return RedirectToAction("Index");
        }

        //TA BORT HATT

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

        //REDIGERA HATT

        [HttpPost]
        public IActionResult UpdateHat(
    string Id,
    string Name,
    string Description,
    double Price,
    int Quantity,
    string ImageUrl,
    IFormFile? ImageFile,
    List<HatMaterialInputViewModel> Materials)
        {
            try
            {
                string imagePath = ImageUrl;

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

                    imagePath = "/images/hats/" + fileName;
                }

                var hatMaterials = Materials?
                    .Where(m => !string.IsNullOrWhiteSpace(m.MaterialId) && m.Amount > 0)
                    .Select(m => new HatMaterial
                    {
                        MaterialId = m.MaterialId,
                        Amount = m.Amount
                    })
                    .ToList() ?? new List<HatMaterial>();

                Hat updatedHat = new Hat
                {
                    Id = Id,
                    Name = Name,
                    Description = Description,
                    Price = Price,
                    Quantity = Quantity,
                    ImageUrl = imagePath,
                    Materials = hatMaterials
                };

                _hatService.UpdateHat(updatedHat);

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
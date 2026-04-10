using Models;
using Microsoft.AspNetCore.Mvc;
using Repository;
using System.Xml;

namespace MVC.Controllers

{
    public class AddUserController : Controller
    {
        private readonly MongoConnector _mongoConnector;

        public AddUserController(MongoConnector mongoConnector)
        {
            _mongoConnector = mongoConnector;
        }
        [HttpGet]
        public IActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(User model)
        {
            if (ModelState.IsValid)
            {
                model.Id = Guid.NewGuid();
                model.SecurityStamp = Guid.NewGuid().ToString();
                model.ConcurrencyStamp = Guid.NewGuid().ToString();
                _mongoConnector.AddUser(model);
                TempData["SuccessMessage"] = "Användaren har lagts till!" ;
                return RedirectToAction("AddUser","AddUser");
            }
            TempData["ErrorMessage"] = "Kunde inte lägga till användaren.";
            return View(model);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}

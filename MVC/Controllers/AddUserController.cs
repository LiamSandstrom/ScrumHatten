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
        public async Task<IActionResult> AddUser(User model, string PasswordHash)
        {
            if (ModelState.IsValid)
            {
                _mongoConnector.AddUser(model);
                return RedirectToAction("Index");
            }


            //        Id = model.Id,
            //        Name = model.Name,
            //        UserName = model.UserName,
            //        Email = model.Email,
            //        PhoneNumber = model.PhoneNumber,
            //        Role = model.Role
            //    };
            //    var result = await _userManager.CreateAsync(user, PasswordHash);
            //    if (result.Succeeded)
            //    {
            //        return RedirectToAction("Index");
            //    }
            //    foreach (var error in result.Errors)
            //    {
            //        ModelState.AddModelError(string.Empty, error.Description);
            //    }
            return View(model);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}

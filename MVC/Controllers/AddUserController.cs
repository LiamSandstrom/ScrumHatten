using Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Xml;

namespace MVC.Controllers

{
    public class AddUserController : Controller
    {
        private readonly UserManager<User> _userManager;

        public AddUserController(UserManager<User> userManager)
        {
            _userManager = userManager;
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
                var user = new User
                {
                    Id = model.Id,
                    Name = model.Name,
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Role = model.Role
                };
                var result = await _userManager.CreateAsync(user, PasswordHash);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}

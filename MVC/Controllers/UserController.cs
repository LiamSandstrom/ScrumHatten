using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    public class UserController : Controller
    {



        [HttpGet]
        public IActionResult UserList()
        {







            return View();
        }

    }
}

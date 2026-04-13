using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;

namespace MVC.Controllers
{
    public class UserController : Controller
    {
        public IActionResult UserList()
        {
            return View();
        }

    }
}

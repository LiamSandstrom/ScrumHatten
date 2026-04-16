using Microsoft.AspNetCore.Mvc;
using Models;

namespace MVC.Controllers
{
    public class CalendarController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public JsonResult GetEvents()
        {
            var events = new List<CalendarEvent>
            {
                new CalendarEvent { Id = 1, Title = "Sprint Planning", Start = DateTime.Now, Color = "#007bff" },
                new CalendarEvent { Id = 2, Title = "Daily Standup", Start = DateTime.Now, Color = "#28a745" }
            };
            return Json(events);
        }
    }
}

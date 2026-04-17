using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Repository;
using DAL.Repositories.Interfaces;
using Models;
using DAL.Repositories;


namespace MVC.Controllers
{
    public class CalendarController : Controller
    {
        private readonly ICalendarRepository _calendarRepository;
        private readonly IUserRepository _collection;


        public CalendarController(ICalendarRepository calendarRepository,IUserRepository collection)
        {
            _calendarRepository = calendarRepository;
            _collection = collection;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SaveEvent([FromBody] CalendarEvent input)
        {
            var currentUserName = User.Identity.Name;

            if (string.IsNullOrEmpty(input.Title))
                return BadRequest("Title is required");

            input.Start = input.Start == default ? DateTime.Now : input.Start;

            input.Color ??= "#007bff";

            if (input.TargetType == "public")
            {
                input.TargetUserNames = new List<string>();
            }
            else if (input.TargetType == "private")
            {
                input.TargetUserNames = new List<string> { currentUserName };
            }
            else if (input.TargetType == "other")
            {
                if (!User.IsInRole("Admin"))
                    return Unauthorized();

                if (input.TargetUserNames == null || !input.TargetUserNames.Any())
                    return BadRequest("No users selected");
            }

            _calendarRepository.AddEvent(input);

            return Json(new { success = true });
        }
        [HttpGet]
        public async Task<JsonResult> GetUsers()
        {
            var users = await _collection.GetAllUsersAsync();
            return Json(users);
        }
        [HttpGet]
        public JsonResult GetEvents()
        {
            var currentUserName = User.Identity.Name;

            var events = _calendarRepository.GetEvents(currentUserName);

            return Json(events);
        }

    }
}

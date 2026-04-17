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

        public CalendarController(ICalendarRepository calendarRepository)
        {
            _calendarRepository = calendarRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetEvents()
        {
            var currentUserID = User.Identity.Name;
            var events = _calendarRepository.GetEvents(currentUserID);
            return Json(events);
        }
        [HttpPost]
        public IActionResult SaveEvent([FromBody] CalendarEvent input)
        {
            var currentUserID = User.Identity.Name;

            if (string.IsNullOrEmpty(input.Title))
                return BadRequest("Title is required");

            input.Start = input.Start == default ? DateTime.Now : input.Start;

            input.End ??= null;
            input.Color ??= "#007bff";

            if (input.TargetType == "public")
            {
                //input.TargetUserName = null;
            }
            else if (input.TargetType == "private")
            {
                //input.TargetUserName = currentUserID;
            }

            _calendarRepository.AddEvent(input);

            return Json(new { success = true });
        }
    }
}

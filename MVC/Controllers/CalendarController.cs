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

            input.Start = input.Start.ToUniversalTime();

            if (input.End != default)
            {
                input.End = input.End.Value.ToUniversalTime();
            }
            input.OwnerName = currentUserName;

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
                    return BadRequest("Ingen användare vald");

                if (!input.TargetUserNames.Contains(currentUserName))
                {
                    input.TargetUserNames.Add(currentUserName);
                }
            }
            if (string.IsNullOrEmpty(input.Id))
            {

                _calendarRepository.AddEvent(input);
            }
            else
            {
                _calendarRepository.UpdateEvent(input);
            }
            return Json(new { success = true });
        }
        [HttpGet]
        public async Task<JsonResult> GetUsers()
        {
            var users = await _collection.GetAllUsersAsync();
            return Json(users);
        }
        [HttpGet]
        public IActionResult GetEvents(bool showAll)
        {
            var currentUserName = User.Identity.Name;

            var events = _calendarRepository.GetEvents(currentUserName, showAll);

            return Json(events);
        }

        [HttpDelete("/Calendar/DeleteEvent/{id}")] // Förtydliga routen för JS-anropet
        public IActionResult DeleteEvent(string id)
        {
            var result = _calendarRepository.DeleteEvent(id);
            if (result) return Ok();
            return BadRequest();
        }
    }

    }



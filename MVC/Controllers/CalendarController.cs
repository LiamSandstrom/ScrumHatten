using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IOrderRepository _orderRepository;


        public CalendarController(ICalendarRepository calendarRepository,IUserRepository collection, IOrderRepository orderRepository)
        {
            _calendarRepository = calendarRepository;
            _collection = collection;
            _orderRepository = orderRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SaveEvent([FromBody] CalendarEvent input)
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
            // If an order is linked, ensure the event title contains the order number
            // and restrict visibility to the maker + the creator so it doesn't show up for others.
            if (!string.IsNullOrEmpty(input.OrderId))
            {
                var order = await _orderRepository.GetOrderByIdAsync(input.OrderId);
                if (order != null)
                {
                    var orderNumber = $"Order: {order.Id.Substring(Math.Max(0, order.Id.Length - 5))}";
                    if (string.IsNullOrEmpty(input.Title) || !input.Title.Contains(orderNumber))
                    {
                        input.Title = string.IsNullOrEmpty(input.Title) ? orderNumber : $"{orderNumber} - {input.Title}";
                    }

                    // Restrict visibility so it does not appear for other users
                    input.TargetType = "private";
                    input.TargetUserNames = new List<string>();
                    if (!string.IsNullOrEmpty(order.MakerName))
                        input.TargetUserNames.Add(order.MakerName);
                    if (!input.TargetUserNames.Contains(currentUserName))
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
        public async Task<IActionResult> GetEvents(bool showAll)
        {
            var currentUserName = User.Identity.Name;

            var events = _calendarRepository.GetEvents(currentUserName, showAll);

            var enriched = new List<CalendarEvent>();
            foreach (var ev in events)
            {
                if (!string.IsNullOrEmpty(ev.OrderId))
                {
                    var order = await _orderRepository.GetOrderByIdAsync(ev.OrderId);
                    if (order != null)
                    {
                        var orderNumber = $"Order: {order.Id.Substring(Math.Max(0, order.Id.Length - 5))}";
                        if (string.IsNullOrEmpty(ev.Title) || !ev.Title.Contains(orderNumber))
                        {
                            ev.Title = string.IsNullOrEmpty(ev.Title) ? orderNumber : $"{orderNumber} - {ev.Title}";
                        }

                        if (!string.IsNullOrEmpty(order.MakerName))
                        {
                            ev.OwnerName = order.MakerName;
                            ev.AssignedToName = order.MakerName;
                        }

                        ev.TargetType = "private";
                        ev.TargetUserNames ??= new List<string>();
                        if (!string.IsNullOrEmpty(order.MakerName) && !ev.TargetUserNames.Contains(order.MakerName))
                            ev.TargetUserNames.Add(order.MakerName);
                        if (!string.IsNullOrEmpty(ev.OwnerName) && !ev.TargetUserNames.Contains(ev.OwnerName))
                            ev.TargetUserNames.Add(ev.OwnerName);
                    }
                }

                enriched.Add(ev);
            }

            return Json(enriched);
        }

        [HttpDelete("/Calendar/DeleteEvent/{id}")]
        public IActionResult DeleteEvent(string id)
        {
            var result = _calendarRepository.DeleteEvent(id);
            if (result) return Ok();
            return BadRequest();
        }



        [HttpGet]
        public async Task<IActionResult> GetMyOrders()
        {
            var identityEmail = User.Identity.Name;

            var allUsers = await _collection.GetAllUsersAsync();
            var currentUser = allUsers.FirstOrDefault(u =>
                u.Email.Equals(identityEmail, StringComparison.OrdinalIgnoreCase));

            if (currentUser == null || string.IsNullOrEmpty(currentUser.Name))
            {
                return Json(new List<object>());
            }

            var ordersById = new List<Order>();
            try
            {
                ordersById = await _orderRepository.GetOrderByMakerIdAsync(currentUser.Id);
            }
            catch
            {
            }

            var ordersByName = (await _orderRepository.GetOrdersByUserAsync(currentUser.Name)).ToList();

            var combined = ordersById.Concat(ordersByName)
                .Where(o => o != null)
                .GroupBy(o => o.Id)
                .Select(g => g.First())
                .ToList();

            var filtered = combined.Where(o => !o.IsDelivered && (o.Status == Status.Pending || o.Status == Status.InProgress));

            var result = filtered.Select(o => new {
                id = o.Id,
                orderNumber = $"Order: {o.Id.Substring(o.Id.Length - 5)}",
                customerName = o.CustomerId ?? "Kund"
            });

            return Json(result);
        }
    }
}




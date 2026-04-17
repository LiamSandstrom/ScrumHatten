using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using MVC.ViewModels;
using Repository;
using Repository.Repositories;
using Services;

namespace MVC.Controllers
{
    [Route("Order")]
    public class OrderController : Controller
    {
        private readonly IOrderRepository orderRepository;
        private IUserRepository userRepository;
        private ICustomerRepository customerRepository;
        private HatRepository hatRepository;

        public OrderController(IUserRepository userRepo, HatRepository hatRepo, ICustomerRepository customerRepo, IOrderRepository orderRepo)
        {
            orderRepository = orderRepo;
            userRepository = userRepo;
            hatRepository = hatRepo;
            customerRepository = customerRepo;
        }

        [HttpGet("")]
        public async Task<IActionResult> Order()
        {
            // Vi skapar temporär data för att simulera databasen
            var mockOrders = new List<Order>
            {
                new Order {
                    Id = "65f1a2b3c4d5e6f7a8b9c001", // Simulerat MongoDB-id
                    DateToFinish = DateTime.Now.AddDays(2),
                    FastOrder = true,
                    Status = Status.Pending,
                    Priority = Priority.High,
                    Hats = new List<Hat> { new Hat(), new Hat(), new Hat() } // 3 hattar
                },
                new Order {
                    Id = "65f1a2b3c4d5e6f7a8b9c002",
                    DateToFinish = DateTime.Now.AddDays(5),
                    FastOrder = false,
                    Status = Status.InProgress,
                    Priority = Priority.Medium,
                    Hats = new List<Hat> { new Hat() } // 1 hatt
                },
                new Order {
                    Id = "65f1a2b3c4d5e6f7a8b9c003",
                    DateToFinish = DateTime.Now.AddDays(1),
                    FastOrder = false,
                    Status = Status.Completed,
                    Priority = Priority.Low,
                    Hats = new List<Hat> { new Hat(), new Hat() } // 2 hattar
                },
                new Order {
                    Id = "65f1a2b3c4d5e6f7a8b9c004",
                    DateToFinish = DateTime.Now.AddDays(10),
                    FastOrder = false,
                    Status = Status.Pending,
                },
                new Order {
                    Id = "65f1a2b3c4d5e6f4", // Simulerat MongoDB-id
                    DateToFinish = DateTime.Now.AddDays(2),
                    FastOrder = true,
                    Status = Status.Pending,
                    Priority = Priority.High,
                    Hats = new List<Hat> { new Hat(), new Hat(), new Hat() } // 3 hattar
                },
                new Order {
                    Id = "65f1a2b3c4d5egrrg57",
                    DateToFinish = DateTime.Now.AddDays(5),
                    FastOrder = false,
                    Status = Status.InProgress,
                    Priority = Priority.Medium,
                    Hats = new List<Hat> { new Hat() } // 1 hatt
                },
                new Order {
                    Id = "65f1a2b3c4d5e6",
                    DateToFinish = DateTime.Now.AddDays(1),
                    FastOrder = false,
                    Status = Status.Completed,
                    Priority = Priority.Low,
                    Hats = new List<Hat> { new Hat(), new Hat() } // 2 hattar
                },
                new Order {
                    Id = "65f1a2b3c4d5e6",
                    DateToFinish = DateTime.Now.AddDays(10),
                    FastOrder = false,
                    Status = Status.Pending,
                    Priority = Priority.Medium,
                    Hats = new List<Hat> { new Hat(), new Hat(), new Hat(), new Hat() } // 4 hattar
                },
                new Order
                {
                    Id = "65f2394795hjf",
                    DateToFinish = DateTime.Now.AddDays(-3),
                    FastOrder = false,
                    Status = Status.Delivered,
                    Priority = Priority.Low,
                    Hats = new List<Hat> { new Hat() } // 1 hatt

                }
            };

            var users = await userRepository.GetAllUsersAsync();
            var customers = await customerRepository.GetAllCustomersAsync();

            var orderViewModel = new OrderViewModel
            {
                OrderList = mockOrders,
                Users = users.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.Name
                }).ToList(),

                Customers = customers.Select(c => new SelectListItem
                {
                    Value = c.Id,
                    Text = c.Name
                }).ToList()
            };

            return View(orderViewModel);
        }

        [HttpGet("Orders/{id}")]
        public async Task<IActionResult> GetOrderById(string id)
        {
            var order = await orderRepository.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound("Ordern hittades inte!");
            return Ok(order);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] Order order)
        {
            if (order == null)
                return BadRequest("Ordern kan inte vara null!");
            await orderRepository.CreateOrderAsync(order);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {

            var users = await userRepository.GetAllUsersAsync();
            var customers = await customerRepository.GetAllCustomersAsync();

            var orderViewModel = new OrderViewModel
            {
                Users = users.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.Name
                }).ToList(),

                Customers = customers.Select(c => new SelectListItem
                {
                    Value = c.Id,
                    Text = c.Name
                }).ToList()
            };


            return View(orderViewModel);
        }

        [HttpGet("GetHatsByType")]
        public IActionResult GetHatsByType(string type)
        {
            if (type == "Stock")
            {
                var hats = hatRepository.GetAllHats();

                return Json(hats.Select(h => new
                {
                    id = h.Id,
                    name = h.Name,
                    price = h.Price,
                    description = h.Description,
                    imageUrl = h.ImageUrl,
                    quantity = h.Quantity
                }));
            }

            if (type == "Custom")
            {
                var hats = new List<Hat>();

                return Json(hats);
            }

            return Json(Array.Empty<object>());
        }
    }
}

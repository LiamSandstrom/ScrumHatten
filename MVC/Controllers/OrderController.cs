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
    public class OrderController : BaseController
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
        public async Task<IActionResult> Create(OrderViewModel model)
        {
            var ignoreFields = new[] { "OrderList", "Users", "Customers", "StockHats", "CustomHats", "Moms" };
            foreach (var field in ignoreFields)
                ModelState.Remove(field);

            if (!ModelState.IsValid)
            {
                return Json(ModelStateErrorResponse("Validering misslyckades"));
            }

            var allHats = await hatRepository.GetAllHats();

            var orderHats = model.Rows
                .Where(r => r.HatId != null)
                .SelectMany(r =>
                {
                    var hat = allHats.FirstOrDefault(h => h.Id == r.HatId);
                    if (hat == null) return Enumerable.Empty<Hat>();
                    return Enumerable.Repeat(hat, r.Quantity);
                })
                .ToList();

            decimal hatSubtotal = orderHats
            .GroupBy(h => h.Id)
            .Sum(g => (decimal)g.First().Price * g.Count());

            decimal subtotalWithTransport = hatSubtotal + model.TransportPrice;
            decimal fastOrderSurcharge = model.FastOrder ? subtotalWithTransport * 0.20m : 0m;
            decimal afterFast = subtotalWithTransport + fastOrderSurcharge;
            decimal finalPrice = afterFast * 1.25m;

            var order = new Order
            {
                FastOrder = model.FastOrder,
                TransportPrice = model.TransportPrice,
                DateToFinish = model.DateToFinish,
                TimeToMake = model.TimeToMake,
                CustomerId = model.SelectedCustomerId,
                MakerId = Guid.TryParse(model.SelectedUserId, out var guid) ? guid : Guid.Empty,
                OrderDate = DateTime.Now,
                Status = Status.Pending,
                Hats = orderHats,
                FinalPrice = finalPrice
            };

            await orderRepository.CreateOrderAsync(order);

            return Json(CreateResponse(true, message: "Order skapad!", notify: true, redirectUrl: "refresh"));
        }

        [HttpGet("GetHatsByType")]
        public async Task<IActionResult> GetHatsByType(string type)
        {
            if (type == "Stock")
            {
                var hats = await hatRepository.GetAllStandardHatsAsync();

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
                var hats = await hatRepository.GetAllCustomHatsAsync();
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

            return Json(Array.Empty<object>());
        }

        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromForm] string id, [FromForm] string status)
        {
            try
            {
                // 1. Konvertera strängen (t.ex. "pending") till din Enum Status
                // 'true' gör att den struntar i om det är stora eller små bokstäver
                if (Enum.TryParse<Status>(status, true, out var parsedStatus))
                {
                    // Här kommer du senare lägga in:
                    // await _orderRepository.UpdateStatusAsync(id, parsedStatus);

                    return Ok(new { message = "Status uppdaterad!" });
                }

                // Om konverteringen misslyckas hamnar vi här
                return BadRequest("Ogiltig status-typ.");
            }
            catch (Exception ex)
            {
                // Om något annat går fel (t.ex. databasfel senare)
                return StatusCode(500, $"Internt serverfel: {ex.Message}");
            }
        }

    }
}

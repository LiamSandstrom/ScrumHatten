using Microsoft.AspNetCore.Mvc;
using Models;
using Repository;
using Services;

namespace MVC.Controllers
{
    [Route("Order")]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet("")]
        public IActionResult Order()
        {
            // Vi skapar temporär data för att simulera databasen
            var mockOrders = new List<Order>
            {
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

            // VIKTIGAST: Vi skickar med listan in i vyn
            return View(mockOrders);
        }

        [HttpGet("Orders/{id}")]
        public async Task<IActionResult> GetOrderById(string id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound("Ordern hittades inte!");
            return Ok(order);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] Order order)
        {
            if (order == null)
                return BadRequest("Ordern kan inte vara null!");
            await _orderRepository.CreateOrderAsync(order);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
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
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

        




        [HttpPost]

        public async Task <IActionResult> UpdateStatus(string id, string status)
        {
            await _orderRepository.UpdateOrderStatusAsync(id, status);
            return Ok("Orderstatus uppdaterad!");
        }

    }
}
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using MongoDB.Driver;
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
    
    var realOrders = await orderRepository.GetAllOrdersAsync();

    
    var users = await userRepository.GetAllUsersAsync();
    var customers = await customerRepository.GetAllCustomersAsync();

    foreach (var order in realOrders)
    {
        if (order.MakerId != Guid.Empty)
        {
            var maker = users.FirstOrDefault(u => u.Id == order.MakerId);
            if (maker != null)
            {
                order.MakerName = maker.Name;
            }
        }
    }

    
    var orderViewModel = new OrderViewModel
    {
        OrderList = realOrders.ToList(),
        
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
            
            var customer= !string.IsNullOrEmpty(order.CustomerId) 
            ? await customerRepository.GetCustomerByIdAsync(order.CustomerId) : null;

            string makerName = "Ingen tilldelad";
    if (order.MakerId != Guid.Empty)
    {
        var user = await userRepository.GetUser(order.MakerId);
        if (user !=null)
                {
                    makerName = user.Name;
                
                }
    }
            
            return Ok(new {
        id = order.Id,
        customer = customer, // Här skickas hela objektet (Name, Email, Adress etc.)
        hats = order.Hats,
        finalPrice = order.FinalPrice,
        makerId = order.MakerId,
        makerName = makerName,
        status = order.Status.ToString()
    });
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

                
                return BadRequest("Ogiltig status-typ.");
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, $"Internt serverfel: {ex.Message}");
            }
        }
     
     
         [HttpPost("AssignToMe")]
        
        public async Task<IActionResult> AssignToMe(string orderId)
        {
            
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userGuid))
            {
                return BadRequest("Du måste vara inloggad för att ta dig an en order.");
            }

            try
            {
                await orderRepository.AssignOrderToMakerAsync(orderId, userGuid);
                return Json(new { 
            success = true, 
            message = "Ordern är nu din och har flyttats till Pågående!" 
        });
    }
    catch (Exception ex) 
    {
        return StatusCode(500, $"Kunde inte ta dig an ordern: {ex.Message}");
            }
        }



[HttpPost("ReleaseOrder")]
public async Task<IActionResult> ReleaseOrder(string orderId)
{
    try 
    {
        // 1. Nolla utföraren (Använder metoden från ditt interface)
        await orderRepository.AssignOrderToMakerAsync(orderId, Guid.Empty);
        
        // 2. Flytta tillbaka till 'Inkommande' status
        await orderRepository.SetStatusAsync(orderId, Status.Pending);

        return Ok();
    }
    catch (Exception ex)
    {
        return BadRequest("Kunde inte släppa ordern: " + ex.Message);
    }
}
    }
}

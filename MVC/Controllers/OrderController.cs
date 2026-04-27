using BL.Services;
using DAL.Repositories;
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MVC.ViewModels;
using Repository;
using Repository.Repositories;


namespace MVC.Controllers
{
    [Route("Order")]
    public class OrderController : BaseController
    {
        private readonly IOrderRepository orderRepository;
        private readonly ICustomsService _customsService;
        private IUserRepository userRepository;
        private ICustomerRepository customerRepository;
        private HatRepository hatRepository;
        private IMaterialRepository materialRepository;

        public OrderController(IUserRepository userRepo, HatRepository hatRepo, ICustomerRepository customerRepo, IOrderRepository orderRepo, IMaterialRepository materialRepo, ICustomsService customsService)
        {
            orderRepository = orderRepo;
            userRepository = userRepo;
            hatRepository = hatRepo;
            customerRepository = customerRepo;
            materialRepository = materialRepo;
            _customsService = customsService;
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

        [HttpGet("GetAllHats")]
        public async Task<IActionResult> GetAllHats()
        {
            try
            {
                var hats = await hatRepository.GetAllHatsWithMaterialsAsync();

                return Json(hats.Select(h => new
                {
                    id = h.Id,
                    name = h.Name,
                    price = h.Price,
                    description = h.Description,
                    imageUrl = h.ImageUrl,
                    quantity = h.Quantity,
                    sizes = h.Sizes,
                    materials = h.Materials
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Json(Array.Empty<object>());
            }
        }


        [HttpGet("Orders/{id}")]
        public async Task<IActionResult> GetOrderById(string id)
        {
            var order = await orderRepository.GetOrderByIdAsync(id);
            if (order == null) return NotFound("Ordern hittades inte!");

            // Hämta kunden för att få adress, e-post etc.
            var customer = !string.IsNullOrEmpty(order.CustomerId)
                ? await customerRepository.GetCustomerByIdAsync(order.CustomerId) : null;

            string makerName = "Ingen tilldelad";
            if (order.MakerId != Guid.Empty)
            {
                var user = await userRepository.GetUser(order.MakerId);
                makerName = user?.Name ?? "Okänd";
            }

            // VIKTIGT: Returnera ALLA fält som båda modalerna behöver
            return Ok(new
            {
                id = order.Id,
                transportPrice = order.TransportPrice,
                timeToMake = order.TimeToMake,
                dateToFinish = order.DateToFinish,
                fastOrder = order.FastOrder,
                finalPrice = order.FinalPrice,
                hats = order.Hats,
                makerId = order.MakerId,
                makerName = makerName,
                customerId = order.CustomerId,
                customer = customer, // Hela objektet för adressuppgifter
                status = order.Status.ToString()
            });
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(OrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(ModelStateErrorResponse("Validering misslyckades"));
            }

            List<Hat> hatsInOrder = new();

            foreach (var hat in model.Rows)
            {
                if (hat.HatId == null)
                {
                    Hat customHat = new Hat
                    {
                        Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                        CustomHat = true,
                        Name = "Custom",
                        Price = (double)hat.CustomPrice,
                        Description = hat.CustomDescription,
                        Materials = hat.Materials,
                        Sizes = new List<HatSize>
                {
                    new HatSize { Label = hat.Size, Quantity = hat.Quantity }
                }
                    };
                    hatsInOrder.Add(customHat);
                    continue;
                }

                var baseHat = await hatRepository.GetHatById(hat.HatId);
                if (baseHat == null)
                    return Json(CreateResponse(false, $"Hatt med id {hat.HatId} hittades inte"));

                if (hat.Materials.Count > 0)
                {
                    var mergedMaterials = baseHat.Materials.Select(bm =>
                    {
                        var extra = hat.Materials.FirstOrDefault(om => om.MaterialId == bm.MaterialId);
                        if (extra != null)
                            return new HatMaterial { MaterialId = bm.MaterialId, Amount = bm.Amount + extra.Amount };
                        return bm;
                    }).ToList();

                    var newMaterials = hat.Materials
                        .Where(om => !baseHat.Materials.Any(bm => bm.MaterialId == om.MaterialId))
                        .ToList();

                    mergedMaterials.AddRange(newMaterials);

                    Hat modifiedHat = new Hat
                    {
                        Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                        CustomHat = true,
                        Name = baseHat.Name,
                        Price = baseHat.Price,
                        Description = baseHat.Description,
                        ImageUrl = baseHat.ImageUrl,
                        Materials = mergedMaterials,
                        Sizes = new List<HatSize>
                {
                    new HatSize { Label = hat.Size, Quantity = hat.Quantity }
                }
                    };
                    hatsInOrder.Add(modifiedHat);
                    continue;
                }

                baseHat.Sizes = new List<HatSize>
        {
            new HatSize { Label = hat.Size, Quantity = hat.Quantity }
        };
                hatsInOrder.Add(baseHat);
            }

            // Price calc
            decimal subtotal = hatsInOrder.Sum(h => (decimal)h.Price * h.Sizes.Sum(s => s.Quantity));
            decimal customs = subtotal * (1 - model.Discount / 100) * (model.Customs / 100);

            decimal finalPrice = CalculatePrice(subtotal, model.Discount, model.Customs, model.FastOrder, model.TransportPrice);
            TimeZoneInfo swedishZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Stockholm");

            var order = new Order
            {
                Hats = hatsInOrder,
                TimeToMake = model.TimeToMake,
                DateToFinish = model.DateToFinish.Date.AddHours(12),
                OrderDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, swedishZone),
                TransportPrice = model.TransportPrice,
                FinalPrice = finalPrice,
                FastOrder = model.FastOrder,
                CustomerId = model.SelectedCustomerId,
                MakerId = Guid.Parse(model.SelectedUserId),
                Status = Status.Pending,
                Priority = Priority.Medium,
                IsDelivered = false,
                Discount = model.Discount,
                CustomsFee = customs
            };

            await orderRepository.CreateOrderAsync(order);

            return Json(CreateResponse(true, "Order skapad", notify: true, redirectUrl: "/Order/"));
        }
        private decimal CalculatePrice(decimal subtotal, decimal discountPercent, decimal customsPercent, bool fastOrder, decimal transportPrice)
        {
            decimal discounted = subtotal * (1 - discountPercent / 100);
            decimal vat = discounted * 0.25m;
            decimal fast = fastOrder ? discounted * 0.20m : 0;
            decimal customs = discounted * (customsPercent / 100);
            return discounted + vat + fast + customs + transportPrice;
        }

        [HttpGet("GetAllMaterials")]
        public async Task<IActionResult> GetAllMaterials()
        {
            try
            {
                var materials = await materialRepository.GetAllMaterialsAsync();
                return Json(materials.Select(m => new
                {
                    id = m.Id,
                    name = m.Name,
                    pricePerUnit = m.PricePerUnit,
                    unit = m.Unit,
                    quantity = m.Quantity
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Json(Array.Empty<object>());
            }
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
                    await orderRepository.SetStatusAsync(id, parsedStatus);
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
                return Json(new
                {
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


                var order = await orderRepository.GetOrderByIdAsync(orderId);
                if (order == null) return NotFound();

                // 2. Nolla BÅDE ID och Namn
                order.MakerId = Guid.Empty;
                order.MakerName = null; // Detta rensar namnet på kortet!
                order.Status = Status.Pending;

                // 3. Spara hela ordern (Använd din Update-metod)
                await orderRepository.UpdateOrderAsync(orderId, order);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Kunde inte släppa ordern: " + ex.Message);
            }
        }

        [HttpPost("UpdateBasicInfo/{id}")]
        public async Task<IActionResult> UpdateBasicInfo(string id, [FromBody] OrderUpdateModel model)
        {
            try
            {
                var order = await orderRepository.GetOrderByIdAsync(id);
                if (order == null) return NotFound();

                // Uppdatera värden - om model-värdet är 0 eller null, kan du välja 
                // att behålla gamla eller skriva över. Här skriver vi över eftersom
                // vi litar på att modalen var förifylld.
                order.TransportPrice = model.TransportPrice;
                order.TimeToMake = model.TimeToMake;

                // Säkerställ att datumet inte är DateTime.MinValue
                if (model.DateToFinish != default)
                {
                    order.DateToFinish = model.DateToFinish.Date.AddHours(12);
                }

                order.CustomerId = model.SelectedCustomerId;
                order.FastOrder = model.FastOrder;

                // Hantera MakerId och MakerName defensivt
                if (!string.IsNullOrEmpty(model.SelectedUserId) && Guid.TryParse(model.SelectedUserId, out Guid makerGuid))
                {
                    order.MakerId = makerGuid;
                    var user = await userRepository.GetUser(makerGuid);
                    order.MakerName = user?.Name;
                }
                else
                {
                    order.MakerId = Guid.Empty;
                    order.MakerName = null;
                }

                // KRASCH-SKYDD: Beräkna priset säkert
                // Vi kollar om Hats är null innan vi kör .Sum()
                decimal hatSubtotal = 0;
                if (order.Hats != null && order.Hats.Any())
                {
                    hatSubtotal = order.Hats.Sum(h => (decimal)(h.Price));
                }

                decimal subtotalWithTransport = hatSubtotal + order.TransportPrice;
                decimal fastOrderSurcharge = order.FastOrder ? subtotalWithTransport * 0.20m : 0m;

                order.FinalPrice = (subtotalWithTransport + fastOrderSurcharge) * 1.25m;

                await orderRepository.UpdateOrderAsync(id, order);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                // Logga felet så du ser exakt vad som hände i Visual Studio
                System.Diagnostics.Debug.WriteLine($"Update Error: {ex.Message}");
                return StatusCode(500, "Internt serverfel vid sparande.");
            }
        }

        [HttpGet("GetCustomsRate")]
        public async Task<IActionResult> GetCustomsRate(string customerId)
        {
            try
            {
                var customer = await customerRepository.GetCustomerByIdAsync(customerId);
                if (customer == null) return NotFound("Kund saknas");

                decimal rate = _customsService.GetCustomsRate(customer.Country);
                return Ok(new
                {
                    customsRate = rate,
                    country = customer.Country
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Kunde inte hämta tullsatsen: {ex.Message}");
            }
        }

        [HttpGet("PrintShippingDocument/{id}")]
        public async Task<IActionResult> PrintShippingDoc(string id)
        {

            var order = await orderRepository.GetOrderByIdAsync(id);
            if (order == null) return NotFound("Ordern hittades inte!");

            var customer = await customerRepository.GetCustomerByIdAsync(order.CustomerId);
            if (customer == null) return NotFound("Kunden hittades inte!");

            return View("ShippingDocument", (order, customer));
        }

        [HttpGet("GetCustomerById")]
        public async Task<IActionResult> GetCustomerById(string id)
        {
            if (string.IsNullOrEmpty(id) || id.Length != 24)
            {
                return BadRequest("Ogiltigt id för kund.");
            }

            var customer = await customerRepository.GetCustomerByIdAsync(id);

            if (customer == null)
            {
                return NotFound($"Ingen kund med ID: {id} hittades.");
            }

            return Ok(customer);
        }

    }


}

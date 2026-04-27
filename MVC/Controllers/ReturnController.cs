
using Microsoft.AspNetCore.Mvc;
using Repository.Repositories;
using MVC.ViewModels;
using Repository;
using MongoDB.Bson;
namespace MVC.Controllers

{
    public class ReturnController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly HatRepository _hatRepository;

        public ReturnController(IOrderRepository orderRepository, HatRepository hatRepository)
        {
            _orderRepository = orderRepository;
            _hatRepository = hatRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitReturn([FromBody] ReturnViewModel model)
        {
            Console.WriteLine(model.ToJson());
            if (!ModelState.IsValid)
            {
                Console.WriteLine("faiaiasddsaldsallsdldssla");
                return BadRequest(ModelState);
            }

            foreach (var hatId in model.HatIds)
            {
                Console.WriteLine($"Updating hat with ID: {hatId} to returned");
                await _orderRepository.UpdateHatReturnedAsync(model.OrderId, hatId, true);
            }
            return Ok("Retur skickad");
        }

        [HttpPost]
        public async Task<IActionResult> SubmitReclaim([FromBody] ReturnViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            foreach (var hatId in model.HatIds)
            {
                await _orderRepository.UpdateHatReclaimedAsync(model.OrderId, hatId, true);
            }

            return Ok("Reklamation skickad");
        }
    }
}

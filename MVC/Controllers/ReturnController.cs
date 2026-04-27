
using Microsoft.AspNetCore.Mvc;
using Repository.Repositories;
using MVC.ViewModels;
using Repository;
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
        public async Task<IActionResult> SubmitReturn(ReturnViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            foreach (var hatId in model.HatIds)
            {
                Console.WriteLine($"Updating hat with ID: {hatId} to returned");
                await _hatRepository.UpdateReturned(hatId, true);
            }
             return Ok("Retur skickad");
        }

        [HttpPost]
        public async Task<IActionResult> SubmitReclaim(ReturnViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            foreach (var hatId in model.HatIds)
            {
                await _hatRepository.UpdateReclaimed(hatId, true);
            }

            return Ok("Reklamation skickad");
        }
    }
}
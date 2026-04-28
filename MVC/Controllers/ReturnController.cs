using Microsoft.AspNetCore.Mvc;
using MVC.ViewModels;
using Repository;
using Models;

namespace MVC.Controllers
{
    public class ReturnController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        public ReturnController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var viewModel = new ReturnIndexViewModel
            {
                ReturnedItems = await _orderRepository.GetAllReturnedHatsAsync(),
                ReclaimedItems = await _orderRepository.GetAllReclaimedHatsAsync()
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsHandled(string orderId, string hatId)
        {
            await _orderRepository.MarkAsHandledAsync(orderId, hatId);
            return RedirectToAction("Index");
        }

        [HttpPost]
public async Task<IActionResult> SubmitReturn([FromBody] ReturnViewModel model)
{
    if (!ModelState.IsValid) return BadRequest(ModelState);

    try 
    {
        await _orderRepository.UpdateReturnReasonAsync(model.OrderId, model.Description);

        foreach (var combinedId in model.HatIds)
        {
            var cleanHatId = combinedId.Split('_')[0]; 
            await _orderRepository.UpdateHatReturnedAsync(model.OrderId, cleanHatId, true);
        }
        return Ok("Retur skickad");
    }
    catch (Exception ex)
    {
        return StatusCode(500, "Fel: " + ex.Message);
    }
}

        [HttpPost]
        public async Task<IActionResult> SubmitReclaim([FromBody] ReturnViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                if (!string.IsNullOrEmpty(model.Description))
                {
                    await _orderRepository.UpdateReturnReasonAsync(model.OrderId, model.Description);
                }

                foreach (var combinedId in model.HatIds)
                {
                    var cleanHatId = combinedId.Split('_')[0];
                    await _orderRepository.UpdateHatReclaimedAsync(model.OrderId, cleanHatId, true);
                }

                return Ok("Reklamation skickad");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SubmitReclaim: {ex.Message}");
                return StatusCode(500, "Ett internt fel uppstod vid bearbetning av reklamationen.");
            }
        }
    }
}
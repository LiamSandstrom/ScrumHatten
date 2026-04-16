using DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Models;
using MVC.ViewModels;
using Repository;

namespace MVC.Controllers
{
    public class MaterialOrderController : Controller
    {
        private readonly IMaterialOrderRepository _materialOrderRepository;
        private readonly IMaterialRepository _materialRepository;

        public MaterialOrderController(
            IMaterialOrderRepository materialOrderRepository,
            IMaterialRepository materialRepository)
        {
            _materialOrderRepository = materialOrderRepository;
            _materialRepository = materialRepository;
        }

        [HttpGet]
        public async Task<IActionResult> MaterialOrder()
        {
            var orders = await _materialOrderRepository.GetAllOrdersAsync();
            var materials = await _materialRepository.GetAllMaterialsAsync();

            ViewBag.Materials = materials;
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateMaterialOrderViewModel model)
        {
            var allMaterials = await _materialRepository.GetAllMaterialsAsync();

            var orderItems = new List<MaterialOrderItem>();
            decimal totalPrice = 0;

            if (model?.Items == null)
            {
                return RedirectToAction("MaterialOrder");
            }

            foreach (var item in model.Items)
            {
                if (string.IsNullOrWhiteSpace(item.MaterialId) || item.Quantity <= 0)
                    continue;

                var material = allMaterials.FirstOrDefault(m => m.Id == item.MaterialId);
                if (material == null)
                    continue;

                orderItems.Add(new MaterialOrderItem
                {
                    MaterialId = material.Id!,
                    MaterialName = material.Name,
                    Quantity = item.Quantity,
                    PricePerUnit = (decimal)material.PricePerUnit
                });

                totalPrice += (decimal)item.Quantity * (decimal)material.PricePerUnit;
            }

            if (!orderItems.Any())
            {
                return RedirectToAction("MaterialOrder");
            }

            var order = new MaterialOrder
            {
                Items = orderItems,
                Status = "Påbörjad",
                TotalPrice = totalPrice,
                CreatedAt = DateTime.Now
            };

            await _materialOrderRepository.AddOrderAsync(order);

            return RedirectToAction("MaterialOrder");
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsDone(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("MaterialOrder");
            }

            var order = await _materialOrderRepository.GetOrderByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status == "Klar")
            {
                return RedirectToAction("MaterialOrder");
            }

            foreach (var item in order.Items)
            {
                await _materialRepository.UpdateQuantityAsync(item.MaterialId, item.Quantity);
            }

            await _materialOrderRepository.UpdateStatusAsync(id, "Klar");

            return RedirectToAction("MaterialOrder");
        }
    }
}
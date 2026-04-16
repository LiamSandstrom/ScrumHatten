using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using MVC.ViewModels;
using Repository;
using Repository.Repositories;

namespace MVC.Controllers
{
    public class OrderController : Controller
    {
        private IUserRepository userRepository;
        private ICustomerRepository customerRepository;
        private HatRepository hatRepository;

        public OrderController(IUserRepository userRepo, HatRepository hatRepo, ICustomerRepository customerRepo)
        {
            userRepository = userRepo;
            hatRepository = hatRepo;
            customerRepository = customerRepo;
        }


        [HttpGet]
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

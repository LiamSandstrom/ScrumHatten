using Microsoft.AspNetCore.Mvc;
using Repository;

namespace MVC.Controllers
{
    [Route("Customer")]
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var customers = await _customerRepository.GetAllCustomersAsync();
            return View(customers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(string id)
        {
            if (string.IsNullOrEmpty(id) || id.Length != 24)
            {
                return BadRequest("Ogiltigt id för kund.");
            }

            var customer = await _customerRepository.GetCustomerByIdAsync(id);

            if (customer == null)
            {
                return NotFound($"Ingen kund med ID: {id} hittades.");
            }

            return Ok(customer);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", await _customerRepository.GetAllCustomersAsync());
            }

            await _customerRepository.AddCustomerAsync(customer);
            TempData["SuccessMessage"] = "Kunden har lagts till!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update(Customer customer)
        {
            if (string.IsNullOrEmpty(customer.Id) || customer.Id.Length != 24)
            {
                return BadRequest("Ogiltigt ID.");
            }

            await _customerRepository.UpdateCustomerAsync(customer);

            TempData["SuccessMessage"] = "Kunduppgifter uppdaterade!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id) || id.Length != 24)
            {
                return BadRequest("Ogiltigt ID.");
            }

            await _customerRepository.DeleteCustomerAsync(id);
            TempData["SuccessMessage"] = "Kunden har raderats.";
            return RedirectToAction(nameof(Index));
        }
    }
}

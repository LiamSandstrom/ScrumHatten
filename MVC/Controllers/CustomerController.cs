using Microsoft.AspNetCore.Mvc;
using MVC.ViewModels.CustomerViewModels;
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






        [HttpGet("CustomerList")]
        public async Task<IActionResult> CustomerList()
        {
            List<Customer> customerList = new List<Customer>();
            customerList = await _customerRepository.GetAllCustomersAsync();


            CustomerListViewModel vm = new CustomerListViewModel()
            {
                allCustomers = customerList
            };

            return View(vm);
        }




        //[HttpGet]
        //public async Task<IActionResult> Index()
        //{
        //    var customers = await _customerRepository.GetAllCustomersAsync();
        //    return View(customers);
        //}

        [HttpGet("Create")]
        public IActionResult Create()
        {
            CreateCustomerViewModel vm = new CreateCustomerViewModel();

            return View(vm);
        }

        [HttpGet("Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            Customer customer = await _customerRepository.GetCustomerByIdAsync(id);

            EditCustomerViewModel vm = new EditCustomerViewModel
            {
                CustomerId = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Adress = customer.Adress,
                ZipCode = customer.ZipCode,
                Country = customer.Country,
            };

            return View(vm);
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(EditCustomerViewModel vm)
        {
            Customer customer = new Customer
            {
                Id = vm.CustomerId,
                Name = vm.Name,
                Email = vm.Email,
                PhoneNumber = vm.PhoneNumber,
                Adress = vm.Adress,
                ZipCode = vm.ZipCode,
                Country = vm.Country,
            };

            await Update(customer);

            return RedirectToAction(nameof(CustomerList));
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



        [HttpPost("CreateCustomer")]
        public async Task<IActionResult> CreateCustomer(CreateCustomerViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", await _customerRepository.GetAllCustomersAsync());
            }

            Customer customer = new Customer
            {

                Name = vm.Name,
                PhoneNumber = vm.PhoneNumber,
                Email = vm.Email,
                Adress = vm.Adress,
                ZipCode = vm.ZipCode,
                Country = vm.Country

            };

            await _customerRepository.AddCustomerAsync(customer);
            TempData["SuccessMessage"] = "Kunden har lagts till!";
            return RedirectToAction(nameof(CustomerList));
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
            return RedirectToAction(nameof(CustomerList));
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
            return RedirectToAction(nameof(CustomerList));
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
            return RedirectToAction(nameof(CustomerList));
        }
    }
}

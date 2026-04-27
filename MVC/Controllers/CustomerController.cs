using Microsoft.AspNetCore.Mvc;
using Models;
using MVC.ViewModels.CustomerViewModels;
using Repository;

namespace MVC.Controllers
{
    [Route("Customer")]
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;

        public CustomerController(ICustomerRepository customerRepository, IOrderRepository orderRepository)
        {
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
        }


        [HttpGet("CustomerList")]
        public async Task<IActionResult> CustomerList(CustomerListViewModel vmIn)

        {


            List<Customer> customerList = new List<Customer>();
            customerList = await _customerRepository.GetAllCustomersAsync();

            List<String> cities = await _customerRepository.GetAllCitiesAsync();
            List<String> countries = await _customerRepository.GetAllCountriesAsync();



            CustomerListViewModel vm = new CustomerListViewModel()
            {
                allCustomers = customerList,
                allCities = cities,
                allCountries = countries
            };

            return View(vm);
        }

        [HttpGet("FilteredCustomerList")]
        public async Task<IActionResult> FilteredCustomerList(string selectedCity, string selectedCountry)
        {

            List<String> cities = await _customerRepository.GetAllCitiesAsync();
            List<String> countries = await _customerRepository.GetAllCountriesAsync();

            if (selectedCity == null & selectedCountry == null)
            {
                return RedirectToAction(nameof(CustomerList));
            }
            else if (selectedCity == null && selectedCountry != null)
            {
                List<Customer> filteredList = await _customerRepository.GetCustomerByCountry(selectedCountry);
                CustomerListViewModel newVm = new CustomerListViewModel
                {
                    allCustomers = filteredList,
                    allCities = cities,
                    allCountries = countries

                };

                return View(nameof(CustomerList), newVm);

            }
            else if (selectedCountry == null && selectedCity != null)
            {
                List<Customer> filteredList = await _customerRepository.GetCustomerByCity(selectedCity);
                CustomerListViewModel newVm = new CustomerListViewModel
                {
                    allCustomers = filteredList,
                    allCities = cities,
                    allCountries = countries
                };

                return View(nameof(CustomerList), newVm);
            }
            else
            {
                List<Customer> filteredListCity = await _customerRepository.GetCustomerByCity(selectedCity);
                List<Customer> filteredListCountry = await _customerRepository.GetCustomerByCountry(selectedCountry);
                List<Customer> combinedList = filteredListCity.Join(
                                                filteredListCountry,
                                                c1 => new { c1.City, c1.Country },
                                                c2 => new { c2.City, c2.Country },
                                                (c1, c2) => c1)
                                                .ToList();

                CustomerListViewModel newVm = new CustomerListViewModel
                {
                    allCustomers = combinedList,
                    allCities = cities,
                    allCountries = countries
                };
                return View(nameof(CustomerList), newVm);




            }
        }

        [HttpGet(nameof(Search))]
        public async Task<IActionResult> Search([FromQuery] string searchTerm)
        {
            // En liten validering för att inte söka efter bara en eller två chars.
            //if (searchTerm != null && searchTerm.Length > 3)
            //{
            List<Customer> result = await _customerRepository.GetCustomerByStringMatch(searchTerm);

            return Json(result);
            //}
            //else
            //{
            //return Json(new { error = "Request too short." });
            //}

        }


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
                City = customer.City,
                Country = customer.Country,
                Discount = customer.Discount,
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
                City = vm.City,
                Country = vm.Country,
                Discount = vm.Discount,
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
                City = vm.City,
                Country = vm.Country,
                Discount = vm.Discount,

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


        public async Task<IActionResult> History(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction(nameof(CustomerList));
            }


            var customer = await _customerRepository.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound($"Ingen finns med kund med id: {id}.");
            }


            List<Order> orders = await _orderRepository.GetOrdersByCustomerIdAsync(id);


            var vm = new HistoryCustomerViewModel
            {
                Id = customer.Id,
                Name = customer.Name,
                Discount = customer.ToString(),
                allOrders = orders.OrderByDescending(o => o.OrderDate).ToList()



            };


            return View(vm);
        }

    }
}

namespace MVC.ViewModels.CustomerViewModels
{
    public class CustomerListViewModel

    {
        public List<Customer> allCustomers { get; set; } = new();

        public List<String> allCities { get; set; } = new();

        public List<String> allCountries { get; set; } = new();

        public string? SelectedCity { get; set; } 

        public string? SelectedCountry { get; set; } 

    }
}

namespace Repository
{
    public interface ICustomerRepository
    {
        Task<Customer> GetCustomerByIdAsync(string id);
        Task<String> GetNameByIdAsync(string id);
        Task<String> GetPhoneNumberByIdAsync(string id);
        Task<String> GetEmailByIdAsync(string id);
        Task<String> GetAdressByIdAsync(string id);
        Task<String> GetZipCodeByIdAsync(string id);
        Task<String> GetCountryByIdAsync(string id);
        Task<List<Customer>> GetAllCustomersAsync();

        Task AddCustomerAsync(Customer customer);
        Task DeleteCustomerAsync(string id);

        Task UpdateCustomerAsync(Customer customer);
    }
}

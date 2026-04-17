using MongoDB.Driver;

namespace Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IMongoCollection<Customer> _collection;

        public CustomerRepository(MongoConnector connector)
        {
            _collection = connector._database.GetCollection<Customer>("Customers");
        }

        public async Task<Customer> GetCustomerByIdAsync(string id)
        {
            var customer = await _collection.Find(c => c.Id == id).FirstOrDefaultAsync();
            return customer;
        }

        public async Task<string> GetNameByIdAsync(string id)
        {
            var name = await _collection
                .Find(c => c.Id == id)
                .Project(c => c.Name)
                .FirstOrDefaultAsync();
            return name;
        }

        public async Task<string> GetPhoneNumberByIdAsync(string id)
        {
            var number = await _collection
                .Find(c => c.Id == id)
                .Project(c => c.PhoneNumber)
                .FirstOrDefaultAsync();
            return number;
        }

        public async Task<string> GetEmailByIdAsync(string id)
        {
            var email = await _collection
                .Find(c => c.Id == id)
                .Project(c => c.Email)
                .FirstOrDefaultAsync();
            return email;
        }

        public async Task<string> GetAdressByIdAsync(string id)
        {
            var adress = await _collection
                .Find(c => c.Id == id)
                .Project(c => c.Adress)
                .FirstOrDefaultAsync();
            return adress;
        }

        public async Task<string> GetZipCodeByIdAsync(string id)
        {
            var zipcode = await _collection
                .Find(c => c.Id == id)
                .Project(c => c.ZipCode)
                .FirstOrDefaultAsync();
            return zipcode;
        }

        public async Task<string> GetCountryByIdAsync(string id)
        {
            var country = await _collection
                .Find(c => c.Id == id)
                .Project(c => c.Country)
                .FirstOrDefaultAsync();
            return country;
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            await _collection.InsertOneAsync(customer);
        }

        public async Task DeleteCustomerAsync(string id)
        {
            var filter = Builders<Customer>.Filter.Eq(c => c.Id, id);
            await _collection.DeleteOneAsync(filter);
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            var filter = Builders<Customer>.Filter.Eq(c => c.Id, customer.Id);
            await _collection.ReplaceOneAsync(filter, customer);
        }



        public async Task<List<String>> GetAllCitiesAsync()
        {


            var citites = await _collection
                .Distinct<string>("City", FilterDefinition<Customer>.Empty)
                .ToListAsync();

            return citites;

        }

        public async Task<List<String>> GetAllCountriesAsync()
        {
            var countries = await _collection
                .Distinct<string>("Country", FilterDefinition<Customer>.Empty)
                .ToListAsync();

            return countries;

        }

        public async Task<List<Customer>> GetCustomerByCity(string city)
        {
            var filter = Builders<Customer>.Filter.Eq(c => c.City, city);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<List<Customer>> GetCustomerByCountry(string country)
        {
            var filter = Builders<Customer>.Filter.Eq(c => c.Country, country);
            return await _collection.Find(filter).ToListAsync();
        }

    }
}

using DAL.Repositories.Interfaces;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Repository;

namespace DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _collection;
        public UserRepository(MongoConnector mc)
        {

            _collection = mc._database.GetCollection<User>("users");
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }


        public async Task<bool> DeleteUserAsync(Guid id)
        {

            var res = await _collection.DeleteOneAsync(u => u.Id == id);
            return res.IsAcknowledged && res.DeletedCount > 0;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            var res = await _collection.ReplaceOneAsync(u => u.Id == user.Id, user);
            return res.IsAcknowledged && res.ModifiedCount > 0;
        }

        public async Task<User> AddUserAsync(User user)
        {
            await _collection.InsertOneAsync(user);
            return user;
        }

        public async Task<User> GetUser(Guid id)
        {
            return await _collection.Find(u => u.Id == id).FirstOrDefaultAsync();
             
        }
        public async Task<List<User>> GetUserByStringMatch(string searchTerm)
        {
        
            if (searchTerm == null)
            {
                searchTerm = string.Empty;
            }
            var query = new BsonRegularExpression(searchTerm, "i");

            var filter = Builders<User>.Filter.Or(
                Builders<User>.Filter.Regex(x => x.Name, query),
                Builders<User>.Filter.Regex(x => x.Email, query),
                Builders<User>.Filter.Regex(x => x.PhoneNumber, query)
                );

            return await _collection.Find(filter).Limit(20).ToListAsync();
        }
    }
}

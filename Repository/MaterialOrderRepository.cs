using Models;
using MongoDB.Driver;

namespace Repository
{
    public class MaterialOrderRepository : IMaterialOrderRepository
    {
        private readonly IMongoCollection<MaterialOrder> _collection;

        public MaterialOrderRepository(MongoConnector connector)
        {
            _collection = connector._database.GetCollection<MaterialOrder>("MaterialOrders");
        }

        public async Task<List<MaterialOrder>> GetAllOrdersAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<MaterialOrder?> GetOrderByIdAsync(string id)
        {
            return await _collection.Find(o => o.Id == id).FirstOrDefaultAsync();
        }

        public async Task AddOrderAsync(MaterialOrder order)
        {
            await _collection.InsertOneAsync(order);
        }

        public async Task UpdateStatusAsync(string id, string newStatus)
        {
            var filter = Builders<MaterialOrder>.Filter.Eq(o => o.Id, id);
            var update = Builders<MaterialOrder>.Update.Set(o => o.Status, newStatus);

            await _collection.UpdateOneAsync(filter, update);
        }
    }
}
using Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IMongoCollection<Order> _collection;
        public OrderRepository(MongoConnector connector)
        {
            _collection = connector._database.GetCollection<Order>("Orders");
        }

        public async Task<Order> GetOrderByIdAsync(string id) =>

            await _collection.Find(o => o.Id == id).FirstOrDefaultAsync();

        public async Task<List<Order>> GetAllOrdersAsync() => 
            await _collection.Find(_ => true).ToListAsync();
        public async Task CreateOrderAsync(Order newOrder)
        {
            await _collection.InsertOneAsync(newOrder);
        }

        public async Task UpdateOrderAsync(string id, Order updatedOrder)
        {
            await _collection.ReplaceOneAsync(o => o.Id == id, updatedOrder);
        }

        public async Task DeleteOrderAsync(string id)
        {
            await _collection.DeleteOneAsync(o => o.Id == id);

        }

        public async Task<List<Order>> GetOrderByStatusAsync(string status)
        {
            return await _collection.Find(o => o.Status == status).ToListAsync();
        }

        public async Task<List<Order>> GetOrderByMakerIdAsync(Guid makerId)
        {
            return await _collection.Find(o => o.MakerId == makerId).ToListAsync();
        }

        public async Task<List<Hat>> GetHatsByOrderIdAsync(string id)
        {
            var order = await GetOrderByIdAsync(id);
            return order?.Hats ?? new List<Hat>();
        }

        public async Task<List<HatMaterial>> GetMaterialsByOrderIdAsync(string id)
        {
            var order = await GetOrderByIdAsync(id);
            return order?.Materials ?? new List<HatMaterial>();
        }

        public async Task<decimal> GetTimeToMakeByOrderIdAsync(string id)
        {
            var order = await GetOrderByIdAsync(id);
            return order?.TimeToMake ?? 0;
        }

        public async Task UpdateOrderStatusAsync(string id, string newStatus)
        {
            var filter = Builders<Order>.Filter.Eq(o => o.Id, id);
            var update = Builders<Order>.Update.Set(o => o.Status, newStatus);
            await _collection.UpdateOneAsync(filter, update);
        }

       
    }


}
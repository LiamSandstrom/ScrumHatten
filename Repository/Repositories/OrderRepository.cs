using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Models;
using MongoDB.Driver;

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

        public async Task<List<Order>> GetOrderByMakerIdAsync(Guid makerId)
        {
            return await _collection.Find(o => o.MakerId == makerId).ToListAsync();
        }

        public async Task<List<Hat>> GetHatsByOrderIdAsync(string id)
        {
            var order = await GetOrderByIdAsync(id);
            return order?.Hats ?? new List<Hat>();
        }

        public async Task<decimal> GetTimeToMakeByOrderIdAsync(string id)
        {
            var order = await GetOrderByIdAsync(id);
            return order?.TimeToMake ?? 0;
        }

        public async Task<Order> GetDeliveredOrdersAsync(string id)
        {
            var filter = Builders<Order>.Filter.And(
                Builders<Order>.Filter.Eq(o => o.Id, id),
                Builders<Order>.Filter.Eq(o => o.Status, Status.Delivered)
            );
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task OrderDateAsync(string id, DateTime newDate)
        {
            var filter = Builders<Order>.Filter.Eq(o => o.Id, id);
            var update = Builders<Order>.Update.Set(o => o.OrderDate, newDate);
            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task SetTransportPriceAsync(string id, decimal newPrice)
        {
            var filter = Builders<Order>.Filter.Eq(o => o.Id, id);
            var update = Builders<Order>.Update.Set(o => o.TransportPrice, newPrice);
            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task SetPriorityAsync(string id, Priority newPriority)
        {
            var filter = Builders<Order>.Filter.Eq(o => o.Id, id);
            var update = Builders<Order>.Update.Set(o => o.Priority, newPriority);
            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task SetStatusAsync(string id, Status newStatus)
        {
            var filter = Builders<Order>.Filter.Eq(o => o.Id, id);
            var update = Builders<Order>.Update.Set(o => o.Status, newStatus);
            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task SetFastOrderAsync(string id, bool isFastOrder)
        {
            var filter = Builders<Order>.Filter.Eq(o => o.Id, id);
            var update = Builders<Order>.Update.Set(o => o.FastOrder, isFastOrder);
            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task UpdatePriorityAsync(string id, Priority newPriority)
        {
            var filter = Builders<Order>.Filter.Eq(o => o.Id, id);
            var update = Builders<Order>.Update.Set(o => o.Priority, newPriority);
            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task UpdateStatusAsync(string id, Status newStatus)
        {
            var filter = Builders<Order>.Filter.Eq(o => o.Id, id);
            var update = Builders<Order>.Update.Set(o => o.Status, newStatus);
            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task UpdateFastOrderAsync(string id, bool isFastOrder)
        {
            var filter = Builders<Order>.Filter.Eq(o => o.Id, id);
            var update = Builders<Order>.Update.Set(o => o.FastOrder, isFastOrder);
            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task SetIsDeliveredAsync(string id, bool isDelivered)
        {
            var filter = Builders<Order>.Filter.Eq(o => o.Id, id);
            var update = Builders<Order>.Update.Set(o => o.IsDelivered, isDelivered);
            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task AssignOrderToMakerAsync(string orderId, Guid makerId)
        {
            var filter = Builders<Order>.Filter.Eq(o => o.Id, orderId);
            var update = Builders<Order>
                .Update.Set(o => o.MakerId, makerId)
                .Set(o => o.Status, Status.InProgress);

            await _collection.UpdateOneAsync(filter, update);
        }
    }
}

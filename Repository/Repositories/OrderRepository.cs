using Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

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
            var update = Builders<Order>.Update.Set(o => o.MakerId, makerId)
            .Set(o => o.Status, Status.InProgress);

            await _collection.UpdateOneAsync(filter, update);
        }

    public async Task<List<Order>> GetOrdersBetweenDates(DateTime startDate, DateTime endDate)
    {
        var builder = Builders<Order>.Filter;

        var filter = builder.And(
            builder.Gte(x => x.OrderDate, startDate),
            builder.Lte(x => x.OrderDate, endDate)
            );

        List<Order> orders = await _collection.Find(filter).ToListAsync();

        return orders;
    }

    public async Task<List<Hat>> GetMostSoldHats(int amountToTake)
    {

        var aggregate = _collection.Aggregate()
            .Unwind("Hats")

            .Group(new BsonDocument
            {
        { "_id", "$Hats._id" },
        { "count", new BsonDocument("$sum", 1) },
        { "hatDoc", new BsonDocument("$first", "$Hats") }
            })

            .Sort(new BsonDocument("count", -1))

            .Limit(amountToTake);

        var results = await aggregate.ToListAsync();

        return results.Select(doc =>
            BsonSerializer.Deserialize<Hat>(doc["hatDoc"].AsBsonDocument)
        ).ToList();
    }

    public async Task<List<Customer>> GetTopCustomers(int amountToTake)
        {
            int topN = 5;

            var pipeline = new[]
            {
    new BsonDocument("$group", new BsonDocument
    {
        { "_id", "$CustomerId" },
        { "TotalSpent", new BsonDocument("$sum", "$FinalPrice") }
    }),

    new BsonDocument("$sort", new BsonDocument("TotalSpent", -1)),

    new BsonDocument("$limit", topN),

    new BsonDocument("$lookup", new BsonDocument
    {
        { "from", "Customers" },
        { "localField", "_id" },
        { "foreignField", "_id" },
        { "as", "CustomerDetails" }
    }),

    new BsonDocument("$unwind", "$CustomerDetails"),

    new BsonDocument("$replaceRoot", new BsonDocument
    {
        { "newRoot", "$CustomerDetails" }
    })
    };

     return await _collection.Aggregate<Customer>(pipeline).ToListAsync();

     }
    
        public async Task<List<Order>> GetOrdersByCustomerIdAsync(string customerid)
        {
            var filter = Builders<Order>.Filter.Eq(o => o.CustomerId, customerid);


            return await _collection.Find(filter).ToListAsync();
        }
    }

}



using Models;
namespace Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IMongoCollection<Order> _collection;
        public OrderRepository(MongoConnector connector)
        {
            _collection = connector._database.GetCollection<Order>("Orders");
        }

        public async Task CreateOrderAsync(Order newOrder)
        {
            await _collection.InsertOneAsync(newOrder);
        }

        public async Task DeleteOrderAsync(string id)
        {
            await _collection.DeleteOneAsync(o => o.Id == id);

        }

        public async Task UpdateOrderAsync(string id, Order updatedOrder)
        {
            await _collection.ReplaceOneAsync(o => o.Id == id, updatedOrder);
        }

        public async Task GetOrderByStatusAsync(string status)
        {
            var orders = await _collection.Find(o => o.Status == status).ToListAsync();
            return orders;
        }

        public async Task GetOrderByMakerIdAsync(Guid makerId)
        {
            var orders = await _collection.Find(o => o.MakerId == makerId).ToListAsync();
            return orders;
        }

        public async Task<decimal> CalculateTotalPriceByOrderIdAsync(string id)
        {
            var order = await _collection.Find(o => o.Id == id).FirstOrDefaultAsync();
            if (order == null) return 0;

            // Summera materialkostnad + transport
            decimal materialCost = order.Materials.Sum(m => m.PricePerUnit * (decimal)m.Quantity);
            decimal subTotal = materialCost + order.TransportPrice;

            // Moms
            return subTotal + order.Moms;
        }
    }


}
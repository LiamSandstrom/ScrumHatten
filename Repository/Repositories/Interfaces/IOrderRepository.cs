using Models;

namespace Repository
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderByIdAsync(string id);
        Task<List<Order>> GetAllOrdersAsync();
        Task<List<Hat>> GetHatsByOrderIdAsync(string id);
        Task<decimal> GetTimeToMakeByOrderIdAsync(string id);
        Task CreateOrderAsync(Order newOrder);
        Task UpdateOrderAsync(string id, Order updatedOrder);
        Task DeleteOrderAsync(string id);
        Task<List<Order>> GetOrderByMakerIdAsync(Guid makerId);

        Task OrderDateAsync(string id, DateTime newDate);


        Task SetTransportPriceAsync(string id, decimal newPrice);

        Task SetPriorityAsync(string id, Priority newPriority);
        Task SetStatusAsync(string id, Status newStatus);
        Task SetFastOrderAsync(string id, bool isFastOrder);

        Task SetIsDeliveredAsync(string id, bool isDelivered);

        Task AssignOrderToMakerAsync(string orderId, Guid makerId);

        Task<List<Order>> GetOrdersByCustomerIdAsync(string customerid);

    }
}
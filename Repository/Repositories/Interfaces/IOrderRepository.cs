using Models;
using MongoDB.Bson.IO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        Task<List<Order>> GetOrdersBetweenDates(DateTime startDate, DateTime endDate);

        Task<List<Hat>> GetMostSoldHats(int amountToTake);

        Task<List<Customer>> GetTopCustomers(int amountToTake);

    }
}
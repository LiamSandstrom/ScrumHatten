using Models;
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
        Task<List<HatMaterial>> GetMaterialsByOrderIdAsync(string id);
        Task<decimal> GetTimeToMakeByOrderIdAsync(string id);
        Task CreateOrderAsync(Order newOrder);
        Task UpdateOrderAsync(string id, Order updatedOrder);
        Task DeleteOrderAsync(string id);
        Task<List<Order>> GetOrderByStatusAsync(string status);
        Task<List<Order>> GetOrderByMakerIdAsync(Guid makerId);

        Task UpdateOrderStatusAsync(string id, string newStatus); 

    }
}
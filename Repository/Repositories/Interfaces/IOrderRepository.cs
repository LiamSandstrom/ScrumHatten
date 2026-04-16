using Models;

namespace Repository
{
    public interface IOrderRepository
    {
        Task<String> GetOrderByIdAsync(string id);
        Task<List<Hat>> GetHatsByOrderIdAsync(string id);
        Task<List<HatMaterial>> GetMaterialsByOrderIdAsync(string id);
        Task<decimal> GetTimeToMakeByOrderIdAsync(string id);
        Task CreateOrderAsync(Order newOrder);
        Task UpdateOrderAsync(string id, Order updatedOrder);
        Task DeleteOrderAsync(string id);
        Task GetOrderByStatusAsync(string status);
        Task GetOrderByMakerIdAsync(Guid makerId);
        Task <decimal> CalculateTotalPriceByOrderIdAsync(string id);
    }
}
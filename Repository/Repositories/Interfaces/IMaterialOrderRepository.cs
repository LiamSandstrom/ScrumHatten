using Models;

namespace Repository
{
    public interface IMaterialOrderRepository
    {
        Task<List<MaterialOrder>> GetAllOrdersAsync();
        Task<MaterialOrder?> GetOrderByIdAsync(string id);
        Task AddOrderAsync(MaterialOrder order);
        Task UpdateStatusAsync(string id, string newStatus);
    }
}
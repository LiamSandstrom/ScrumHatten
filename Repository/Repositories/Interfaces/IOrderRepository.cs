using Models;
using MongoDB.Driver;

namespace Repository
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderByIdAsync(string id);

        Task UpdateHatReclaimedAsync(string orderId, string hatId, bool isReclaimed);

        Task UpdateHatReturnedAsync(string orderId, string hatId, bool isReturned);

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

        Task<List<Order>> GetOrdersByCustomerIdAsync(string customerid);

        Task<List<SalesMonth>> GetOrdersByMonth(DateTime startDate, DateTime endDate);

        Task<IEnumerable<Order>> GetOrdersByUserAsync(string userName);
        Task<List<ReturnItemDto>> GetAllReturnedHatsAsync();
        Task<List<ReturnItemDto>> GetAllReclaimedHatsAsync();
        Task MarkAsHandledAsync(string orderId, string hatId);
        Task UpdateReturnReasonAsync(string orderId, string reason);
    }
}

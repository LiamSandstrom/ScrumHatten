using Models;

namespace MVC.ViewModels.CustomerViewModels
{
    public class HistoryCustomerViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string Discount { get; set; }
        public List<Order> allOrders { get; set; } = new();
    }
}

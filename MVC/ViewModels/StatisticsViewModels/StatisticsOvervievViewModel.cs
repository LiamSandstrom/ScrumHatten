using Models;

namespace MVC.ViewModels.StatisticsViewModels
{
    public class StatisticsOverviewViewModel
    {
        public List<Order> PastYearsOrders { get; set; } = new();

        public List<Order> ThisYearsOrders { get; set; } = new();

        public List<Order> PastMonthsOrders { get; set; } = new();

        public List<Order> PastWeeksOrders { get; set; } = new();

        public List<Order> PastDaysOrders { get; set; } = new();

        public List<Order> ThisDaysOrders { get; set; } = new();

        public List<Hat> TopHats { get; set; } = new();

        public List<Customer> TopCustomers { get; set; } = new();
    }
}
using Microsoft.AspNetCore.Mvc;
using Models;
using MVC.ViewModels.StatisticsViewModels;
using Repository;
using static System.Net.WebRequestMethods;

namespace MVC.Controllers
{
    public class StatisticsController : Controller

    {

        IOrderRepository _orderRepository;
        public StatisticsController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IActionResult> Overview()
        {

            DateTime today = DateTime.Now;
            DateTime todayOnlyDate = DateTime.Today;


            // Orders
            DateTime endDate = DateTime.Today.AddDays(1).AddTicks(-1);
            List<Order> thisDaysOrders = await _orderRepository.GetOrdersBetweenDates(todayOnlyDate, today);

            DateTime oneDayAgo = todayOnlyDate.AddHours(-24);
            List<Order> pastDaysOrders = await _orderRepository.GetOrdersBetweenDates(oneDayAgo, today);

            DateTime oneWeekAgo = todayOnlyDate.AddDays(-7);
            List<Order> pastWeeksOrders = await _orderRepository.GetOrdersBetweenDates(oneWeekAgo, today);

            DateTime oneMonthAgo = todayOnlyDate.AddMonths(-1);
            List<Order> pastMonthsOrders = await _orderRepository.GetOrdersBetweenDates(oneMonthAgo, today);

            DateTime oneYearAgo = todayOnlyDate.AddYears(-1);
            List<Order> pastYearsOrders = await _orderRepository.GetOrdersBetweenDates(oneYearAgo, today);

            DateTime firstDayOfThisYear = new DateTime(DateTime.Now.Year, 1, 1);
            List<Order> thisYearsOrders = await _orderRepository.GetOrdersBetweenDates(firstDayOfThisYear, today);

            List<Hat> topFiveHats = await _orderRepository.GetMostSoldHats(5);

            List<Customer> topFiveCustomers = await _orderRepository.GetTopCustomers(5);




            StatisticsOverviewViewModel vm = new StatisticsOverviewViewModel
            {
                ThisDaysOrders = thisDaysOrders,
                PastDaysOrders = pastDaysOrders,
                PastWeeksOrders = pastWeeksOrders,
                PastMonthsOrders = pastMonthsOrders,
                PastYearsOrders = pastYearsOrders,
                ThisYearsOrders = thisYearsOrders,
                TopHats = topFiveHats,
            };



            return View(vm);
        }


        public async Task<IActionResult> SalesData([FromQuery] int monthsBack)
        {
            DateTime endDate = DateTime.Now.Date;
            DateTime startDate = endDate.AddMonths(-monthsBack).Date;

            List<SalesMonth> salesMonths = await _orderRepository.GetOrdersByMonth(startDate, endDate);

            return Json(salesMonths);

        }
    }
}
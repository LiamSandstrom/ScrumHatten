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


            DateTime endDate = DateTime.Today.AddDays(1).AddTicks(-1);
            DateTime oneDayAgo = todayOnlyDate.AddHours(-24);
            DateTime oneWeekAgo = todayOnlyDate.AddDays(-7);
            DateTime oneMonthAgo = todayOnlyDate.AddMonths(-1);
            DateTime oneYearAgo = todayOnlyDate.AddYears(-1);
            DateTime firstDayOfThisYear = new DateTime(DateTime.Now.Year, 1, 1);

            var thisDaysTask = _orderRepository.GetOrdersBetweenDates(todayOnlyDate, today);
            var pastDaysTask = _orderRepository.GetOrdersBetweenDates(oneDayAgo, today);
            var pastWeeksTask = _orderRepository.GetOrdersBetweenDates(oneWeekAgo, today);
            var pastMonthsTask = _orderRepository.GetOrdersBetweenDates(oneMonthAgo, today);
            var pastYearsTask = _orderRepository.GetOrdersBetweenDates(oneYearAgo, today);
            var thisYearsTask = _orderRepository.GetOrdersBetweenDates(firstDayOfThisYear, today);
            var topHatsTask = _orderRepository.GetMostSoldHats(5);
            var topCustomersTask = _orderRepository.GetTopCustomers(5);

            await Task.WhenAll(thisDaysTask, pastDaysTask, pastWeeksTask, pastMonthsTask, pastYearsTask, thisYearsTask, topHatsTask, topCustomersTask);

            List<Order> thisDaysOrders = await thisDaysTask;
            List<Order> pastDaysOrders = await pastDaysTask;
            List<Order> pastWeeksOrders = await pastWeeksTask;
            List<Order> pastMonthsOrders = await pastMonthsTask;
            List<Order> pastYearsOrders = await pastYearsTask;
            List<Order> thisYearsOrders = await thisYearsTask;
            List<Hat> topFiveHats = await topHatsTask;
            List<Customer> topFiveCustomers = await topCustomersTask;


            StatisticsOverviewViewModel vm = new StatisticsOverviewViewModel
            {
                ThisDaysOrders = thisDaysOrders,
                PastDaysOrders = pastDaysOrders,
                PastWeeksOrders = pastWeeksOrders,
                PastMonthsOrders = pastMonthsOrders,
                PastYearsOrders = pastYearsOrders,
                ThisYearsOrders = thisYearsOrders,
                TopHats = topFiveHats,
                TopCustomers = topFiveCustomers
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
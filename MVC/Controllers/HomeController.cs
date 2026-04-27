using DAL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using MVC.Models;
using MVC.ViewModels.StatisticsViewModels;
using Repository;
using System.Diagnostics;

namespace MVC.Controllers;

public class HomeController : Controller
{
    IOrderRepository _orderRepository;
    IMaterialRepository _materialRepoistory;
    public HomeController(IOrderRepository orderRepository, IMaterialRepository materialRepository)
    {
        _orderRepository = orderRepository;
        _materialRepoistory = materialRepository;
    }
    public async Task<IActionResult> Index()
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return RedirectToAction("Login", "Account");
        }

        DateTime today = DateTime.Now;
        DateTime todayOnlyDate = DateTime.Today;

        // Orders
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
        };

        return View(vm);
    }
    //public IActionResult Privacy()
    //{
    //    return View();
    //}

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public async Task<IActionResult> LowInventoryAlerts()
    {

        List<Material> materials = await _materialRepoistory.GetLowInventoryMaterials();

        return Json(materials);
    }

}

//jippiii


// hej
// hej
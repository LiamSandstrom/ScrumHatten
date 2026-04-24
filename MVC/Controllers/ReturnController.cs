using BL.Services;
using DAL.Repositories;
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using MongoDB.Driver;
using MVC.ViewModels;
using Repository;
using Repository.Repositories;
using Services;

namespace MVC.Controllers
{
    public class ReturnController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        public ReturnController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}

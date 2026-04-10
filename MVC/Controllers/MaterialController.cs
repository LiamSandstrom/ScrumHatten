using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Repository;

namespace MVC.Controllers { 

    public class MaterialController : Controller
    {
        public IActionResult Material()
        {
            return View();
        }
        private readonly IMaterialRepository _materialRepository;

        [HttpGet("names")]
        public async Task<IActionResult> GetNames()
        {
            var names = await _materialRepository.GetAllMaterialsAsync();
            return Ok(names);
        }

        [HttpGet("unit")]
        public async Task<IActionResult> GetUnitById(string id)
        {
            var unit = await _materialRepository.GetUnitByIdAsync(id);
            return Ok(unit);
        }

        [HttpGet("quantity")]
        public async Task<IActionResult> GetQuantityById(string id)
        {
            var quantity = await _materialRepository.GetQuantityByIdAsync(id);
            return Ok(quantity);
        }

        [HttpGet("price")] 
        public async Task<IActionResult> GetPriceById(string id)
            {
            var price = await _materialRepository.GetPriceByIdAsync(id);
            return Ok(price);
        }

    }
}

//Task<String> GetUnitByIdAsync(string id);
//Task<int> GetQuantityByIdAsync(string id);
//Task<double> GetPriceByIdAsync(string id);
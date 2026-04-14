using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Repository;

namespace MVC.Controllers
{
    public class MaterialController : Controller
    {
        private readonly IMaterialRepository _materialRepository;

        public MaterialController(IMaterialRepository materialRepository)
        {
            _materialRepository = materialRepository;
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var material = await _materialRepository.GetMaterialByIdAsync(id);
            if (material == null)
                return NotFound("Materialet hittades inte!");
            return Ok(material);
        }

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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Material material)
        {
            if (material == null)
                return BadRequest("Material-data saknas!");
            await _materialRepository.AddMaterialAsync(material);

            return CreatedAtAction(nameof(GetById), new { id = material.Id }, material);
        }

        public async Task<IActionResult> Material()
        {
            var materials = await _materialRepository.GetAllMaterialsAsync();
            return View(materials);
        }

        [HttpPatch("Material/Restock/{id}")]
        public async Task<IActionResult> Restock(string id, [FromBody] double addedAmount)
        {
            try
            {
                await _materialRepository.UpdateQuantityAsync(id, addedAmount);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}

//Task<String> GetUnitByIdAsync(string id);
//Task<int> GetQuantityByIdAsync(string id);
//Task<double> GetPriceByIdAsync(string id);

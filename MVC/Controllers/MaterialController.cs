using System;
using System.Collections.Generic;
using System.Text;
using DAL.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{

[Route("Material")]
    public class MaterialController : Controller
    {
        private readonly IMaterialRepository _materialRepository;

        public MaterialController(IMaterialRepository materialRepository)
        {
            _materialRepository = materialRepository;
        }

        [HttpGet("/Material/details/{id}")]
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
            return View("Material",materials);
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

        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromBody] Material updatedMaterial)
        {
            if (updatedMaterial == null || string.IsNullOrEmpty(updatedMaterial.Id))
            {
                return BadRequest("Ogiltig materialdata!");
            }
            try
            {
                await _materialRepository.UpdateNameAsync(updatedMaterial.Id, updatedMaterial.Name);
                await _materialRepository.UpdatePricePerUnitAsync(updatedMaterial.Id, updatedMaterial.PricePerUnit);
                await _materialRepository.UpdateUnitAsync(updatedMaterial.Id, updatedMaterial.Unit);
                await _materialRepository.ReplaceQuantityAsync(updatedMaterial.Id, updatedMaterial.Quantity);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);

            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _materialRepository.DeleteMaterialAsync(id);
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

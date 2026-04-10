using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Repository;

namespace MVC.Controllers { 

    public class MaterialController : ControllerBase
    {
        private readonly IMaterialRepository _materialRepository;

        [HttpGet("names")]
        public async Task<IActionResult> GetNames()
        {
            var names 
        }

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Models;

namespace Repository
{
    public interface IMaterialRepository
    {
        Task<String> GetUnitByIdAsync(string id);
        Task<int> GetQuantityByIdAsync(string id);
        Task<double> GetPriceByIdAsync(string id);
        Task<List<string>> GetAllMaterialsAsync();

        Task AddMaterialAsync(Material material);
        Task<Material> GetMaterialByIdAsync (string id);
        
    }
}

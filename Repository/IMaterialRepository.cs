using System;
using System.Collections.Generic;
using System.Text;
using Models;

namespace DAL.Repositories
{
    public interface IMaterialRepository
    {
        Task<String> GetUnitByIdAsync(string id);
        Task<double> GetQuantityByIdAsync(string id);
        Task<double> GetPriceByIdAsync(string id);
        Task<List<Material>> GetAllMaterialsAsync();

        Task AddMaterialAsync(Material material);
        Task<Material> GetMaterialByIdAsync (string id);

        Task UpdateQuantityAsync(string id, double addedAmount);
        Task UpdateNameAsync(string id, string newName);
        Task UpdatePricePerUnitAsync(string id, double newPrice);
        Task UpdateUnitAsync(string id, string newUnit);

        Task UpdateLowInventoryWarningPoint(string id, double newPoint);
        Task DeleteMaterialAsync(string id);
        Task ReplaceQuantityAsync(string id, double newQuantity);
        Task<List<Material>> GetLowInventoryMaterials();
        
    }
}

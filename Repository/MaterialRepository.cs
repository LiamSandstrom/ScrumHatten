using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using Models;
using System.Security.Cryptography.X509Certificates;

namespace Repository
{
    public class MaterialRepository : IMaterialRepository
    {
        private readonly IMongoCollection<Material> _collection;
        public MaterialRepository(MongoConnector connector) {
            
            _collection = connector._database.GetCollection<Material>("Materials");
        }
        public async Task<double>GetPriceByIdAsync(string id)
                    {
            var price = await _collection.Find(m => m.Id == id)
                .Project(m => m.PricePerUnit)
                .FirstOrDefaultAsync();
            return price;
        }

        public async Task<int> GetQuantityByIdAsync(string id)
        {
            var quantity = await _collection.Find(m => m.Id == id)
                .Project(m => m.Quantity)
                .FirstOrDefaultAsync();
            return quantity;
        }

        public async Task<string> GetUnitByIdAsync(string id)
        {
            var unit = await _collection.Find(m => m.Id == id)
                .Project(m => m.Unit)
                .FirstOrDefaultAsync();
            return unit;
        }
        public async Task<List<Material>> GetAllMaterialsAsync()
        {
           return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<Material> GetMaterialByIdAsync(string id)
        {
            var material = await _collection.Find(m => m.Id == id).FirstOrDefaultAsync();
            return material;
        }

        

        public async Task AddMaterialAsync(Material newMaterial)
        {
            await _collection.InsertOneAsync(newMaterial);
        }
    }
}

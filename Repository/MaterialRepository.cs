using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using Models;
using System.Security.Cryptography.X509Certificates;
using DAL.Repositories;

namespace Repository
{
    public class MaterialRepository : IMaterialRepository
    {
        private readonly IMongoCollection<Material> _collection;
        public MaterialRepository(MongoConnector connector)
        {

            _collection = connector._database.GetCollection<Material>("Materials");
        }
        public async Task<double> GetPriceByIdAsync(string id)
        {
            var price = await _collection.Find(m => m.Id == id)
                .Project(m => m.PricePerUnit)
                .FirstOrDefaultAsync();
            return price;
        }

        public async Task<double> GetQuantityByIdAsync(string id)
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


        public async Task UpdateQuantityAsync(string id, double addedAmount)

        {
            var filter = Builders<Material>.Filter.Eq(m => m.Id, id);
            var update = Builders<Material>.Update.Inc(m => m.Quantity, addedAmount);

            await _collection.UpdateOneAsync(filter, update);

        }

        public async Task ReplaceQuantityAsync(string id, double newQuantity)
        {
            var filter = Builders<Material>.Filter.Eq(m => m.Id, id);
            var update = Builders<Material>.Update.Set(m => m.Quantity, newQuantity);

            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task UpdateNameAsync(string id, string newName)
        {
            var filter = Builders<Material>.Filter.Eq(m => m.Id, id);
            var update = Builders<Material>.Update.Set(m => m.Name, newName);

            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task UpdatePricePerUnitAsync(string id, double newPrice)
        {
            var filter = Builders<Material>.Filter.Eq(m => m.Id, id);
            var update = Builders<Material>.Update.Set(m => m.PricePerUnit, newPrice);

            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task UpdateUnitAsync(string id, string newUnit)
        {
            var filter = Builders<Material>.Filter.Eq(m => m.Id, id);
            var update = Builders<Material>.Update.Set(m => m.Unit, newUnit);

            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task DeleteMaterialAsync(string id)
        {
            var filter = Builders<Material>.Filter.Eq(m => m.Id, id);
            await _collection.DeleteOneAsync(filter);





        }

    }
}

using Models;
using MongoDB.Driver;
using DAL.Repositories;

namespace Repository.Repositories
{
    public class HatRepository
    {
        private readonly IMongoCollection<Hat> _hatCollection;
        private readonly IMongoCollection<Material> _materialCollection;

        public HatRepository(MongoConnector mongoConnector)
        {
            _hatCollection = mongoConnector._database.GetCollection<Hat>("Hats");
            _materialCollection = mongoConnector._database.GetCollection<Material>("Materials");
        }

        public async Task<List<Hat>> GetAllHats()
        {
            try
            {
                return await _hatCollection
                    .Find(_ => true)
                    .Project<Hat>(Builders<Hat>.Projection.Exclude(h => h.ImageBase64))
                    .ToListAsync();
            }
             catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
        throw;
    }
        }

        public async Task<List<Hat>> GetAllCustomHatsAsync()
        {
            var customHats = await _hatCollection
                .Find(h => h.CustomHat == true)
                .Project<Hat>(Builders<Hat>.Projection.Exclude(h => h.ImageBase64))
                .ToListAsync();
            return customHats;
        }

        public async Task<List<Hat>> GetAllStandardHatsAsync()
        {
            var standardHats = await _hatCollection
                .Find(h => h.CustomHat != true)
                .Project<Hat>(Builders<Hat>.Projection.Exclude(h => h.ImageBase64))
                .ToListAsync();
            return standardHats;
        }

        public async Task<HatWithMaterial> GetHatWithMaterialsAsync(string id)
        {
            var hat = await _hatCollection.Find(h => h.Id == id).FirstOrDefaultAsync();
            if (hat == null) return null;

            var materialIds = hat.Materials.Select(m => m.MaterialId).ToList();

            var materials = await _materialCollection
                .Find(m => materialIds.Contains(m.Id))
                .ToListAsync();

            var materialDict = materials.ToDictionary(m => m.Id);

            return new HatWithMaterial
            {
                Id = hat.Id,
                Name = hat.Name,
                Price = hat.Price,
                Description = hat.Description,
                ImageUrl = hat.ImageUrl,
                CustomHat = hat.CustomHat,
                Materials = hat.Materials
                    .Where(m => materialDict.ContainsKey(m.MaterialId))
                    .Select(m => new HatMaterialDetail
                    {
                        MaterialId = m.MaterialId,
                        Amount = m.Amount,
                        Name = materialDict[m.MaterialId].Name,
                        Quantity = materialDict[m.MaterialId].Quantity,
                        PricePerUnit = materialDict[m.MaterialId].PricePerUnit,
                        Unit = materialDict[m.MaterialId].Unit
                    }).ToList()
            };
        }
        public async Task<List<HatWithMaterial>> GetAllHatsWithMaterialsAsync()
        {
            var hats = await _hatCollection.Find(_ => true).ToListAsync();

            var allMaterials = hats.SelectMany(h => h.Materials.Select(m => m.MaterialId))
                .Distinct()
                .ToList();

            var materials = await _materialCollection.Find(m => allMaterials.Contains(m.Id)).ToListAsync();

            var materialDict = materials.ToDictionary(m => m.Id);

            return hats.Select(hat => new HatWithMaterial
            {
                Id = hat.Id,
                Name = hat.Name,
                Price = hat.Price,
                Description = hat.Description,
                ImageUrl = hat.ImageUrl,
                CustomHat = hat.CustomHat,
                Materials = hat.Materials
                    .Where(m => materialDict.ContainsKey(m.MaterialId))
                    .Select(m => new HatMaterialDetail
                    {
                        MaterialId = m.MaterialId,
                        Amount = m.Amount,
                        Name = materialDict[m.MaterialId].Name,
                        Quantity = materialDict[m.MaterialId].Quantity,
                        PricePerUnit = materialDict[m.MaterialId].PricePerUnit,
                        Unit = materialDict[m.MaterialId].Unit
                    }).ToList()
            }).ToList();
        }

        public async Task AddHat(Hat hat)
        {
            await _hatCollection.InsertOneAsync(hat);
        }

        public async Task<Hat?> GetHatById(string id)
        {
            return await _hatCollection.Find(h => h.Id == id).FirstOrDefaultAsync();
        }

        public async Task DeleteHat(string id)
        {
            await _hatCollection.DeleteOneAsync(h => h.Id == id);
        }

        public async Task UpdateHat(Hat hat)
        {
            await _hatCollection.ReplaceOneAsync(h => h.Id == hat.Id, hat);
        }
    }
}

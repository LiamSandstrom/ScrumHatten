using Models;
using MongoDB.Driver;

namespace Repository.Repositories
{
    public class HatRepository
    {
        private readonly IMongoCollection<Hat> _hatCollection;

        public HatRepository(MongoConnector mongoConnector)
        {
            _hatCollection = mongoConnector._database.GetCollection<Hat>("Hats");
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
            catch
            {
                return new List<Hat>();
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

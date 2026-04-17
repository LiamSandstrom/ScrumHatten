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

        public List<Hat> GetAllHats()
        {
            try
            {
                return _hatCollection.Find(_ => true).ToList();
            }
            catch
            {
                return new List<Hat>();
            }
        }

        public async Task<List<Hat>> GetAllCustomHatsAsync()
        {
            var customHats = await _hatCollection.Find(h => h.CustomHat == true).ToListAsync();
            return customHats;
        }

        public async Task<List<Hat>> GetAllStandardHatsAsync()
        {
            var standardHats = await _hatCollection.Find(h => h.CustomHat == false).ToListAsync();
            return standardHats;
        }

        public void AddHat(Hat hat)
        {
            _hatCollection.InsertOne(hat);
        }

        public Hat? GetHatById(string id)
        {
            return _hatCollection.Find(h => h.Id == id).FirstOrDefault();
        }

        public void DeleteHat(string id)
        {
            _hatCollection.DeleteOne(h => h.Id == id);
        }

        public void UpdateHat(Hat hat)
        {
            _hatCollection.ReplaceOne(h => h.Id == hat.Id, hat);
        }
    }
}

